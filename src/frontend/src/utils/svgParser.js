/**
 * svgParser.js
 *
 * Parses an SVG file/string into the seatmap JSON DTO structure expected by the backend.
 *
 * Rules (TicketHub Seatmap Design Handbook):
 *  - Zone groups are identified by their `id` attribute, NOT by depth position:
 *      id="STAGE"   → Stage zone (isStage=true, isSalable=false)
 *      id="Zone-*"  → Seat zone  (isStage=false, isSalable=true)
 *  - Zones can be anywhere in the SVG tree (direct children of <svg> OR nested inside
 *    a wrapper group like Figma's "Group 1"). The parser searches all depths.
 *  - Inside a zone group:
 *      <circle id="RowLabel_SeatNumber">  →  Seat  (e.g. A_1, B_12)
 *      <path>                             →  svgElement type="path"
 *      <rect>                             →  svgElement type="path" (converted to path data)
 *      <text>                             →  svgElement type="text"
 *      nested <g>                         →  flattened one level deep
 */

// ─── Helpers ──────────────────────────────────────────────────────────────────

function parseAttr(el, attr, defaultVal = null) {
  const val = el.getAttribute(attr)
  return val !== null && val !== '' ? val : defaultVal
}

function parseFloat2(val, defaultVal = 0) {
  const n = parseFloat(val)
  return isNaN(n) ? defaultVal : n
}

function hasGradientFill(value) {
  return typeof value === 'string' && value.trim().startsWith('url(')
}

function isStageZone(id) {
  return id === 'STAGE' || id.toLowerCase() === 'stage'
}

function isZoneGroup(el) {
  const id = (el.getAttribute('id') || '').trim()
  return isStageZone(id) || id.startsWith('Zone-')
}

// ─── Bounding box ─────────────────────────────────────────────────────────────

/**
 * Compute the bounding box of all visual children (circles, rects).
 * Paths are hard to compute without a geometry library, so we use rect/circle only.
 */
function computeBoundingBox(elements) {
  let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity

  function expand(x1, y1, x2, y2) {
    minX = Math.min(minX, x1); minY = Math.min(minY, y1)
    maxX = Math.max(maxX, x2); maxY = Math.max(maxY, y2)
  }

  elements.forEach(el => {
    const tag = el.tagName.toLowerCase()
    if (tag === 'circle') {
      const cx = parseFloat2(el.getAttribute('cx'))
      const cy = parseFloat2(el.getAttribute('cy'))
      const r  = parseFloat2(el.getAttribute('r'))
      expand(cx - r, cy - r, cx + r, cy + r)
    } else if (tag === 'rect') {
      const x = parseFloat2(el.getAttribute('x'))
      const y = parseFloat2(el.getAttribute('y'))
      const w = parseFloat2(el.getAttribute('width'))
      const h = parseFloat2(el.getAttribute('height'))
      expand(x, y, x + w, y + h)
    } else if (tag === 'text') {
      const x  = parseFloat2(el.getAttribute('x'))
      const y  = parseFloat2(el.getAttribute('y'))
      const fs = parseFloat2(el.getAttribute('font-size'), 12)
      expand(x, y - fs, x + 120, y)
    }
  })

  if (minX === Infinity) return { x: 0, y: 0, width: 0, height: 0 }
  return {
    x: Math.round(minX),
    y: Math.round(minY),
    width:  Math.round(maxX - minX),
    height: Math.round(maxY - minY)
  }
}

// ─── Element parsers ───────────────────────────────────────────────────────────

function parsePath(el) {
  return {
    type: 'path',
    x: parseFloat2(el.getAttribute('x')),
    y: parseFloat2(el.getAttribute('y')),
    width:  parseFloat2(el.getAttribute('width')),
    height: parseFloat2(el.getAttribute('height')),
    fill:   parseAttr(el, 'fill', ''),
    stroke: parseAttr(el, 'stroke', ''),
    strokeWidth: parseFloat2(el.getAttribute('stroke-width'), 0),
    data: parseAttr(el, 'd', ''),
    text: null, fontSize: 0, fontFamily: ''
  }
}

/**
 * Convert <rect x y width height> to a path element.
 * Rect is the most common zone-background element in Figma exports.
 */
function parseRect(el) {
  const x  = parseFloat2(el.getAttribute('x'))
  const y  = parseFloat2(el.getAttribute('y'))
  const w  = parseFloat2(el.getAttribute('width'))
  const h  = parseFloat2(el.getAttribute('height'))
  const rx = parseFloat2(el.getAttribute('rx'), 0)

  // Build SVG path data for a (possibly rounded) rectangle
  let d
  if (rx > 0) {
    d = `M ${x + rx} ${y} L ${x + w - rx} ${y} Q ${x + w} ${y} ${x + w} ${y + rx} `
      + `L ${x + w} ${y + h - rx} Q ${x + w} ${y + h} ${x + w - rx} ${y + h} `
      + `L ${x + rx} ${y + h} Q ${x} ${y + h} ${x} ${y + h - rx} `
      + `L ${x} ${y + rx} Q ${x} ${y} ${x + rx} ${y} Z`
  } else {
    d = `M ${x} ${y} L ${x + w} ${y} L ${x + w} ${y + h} L ${x} ${y + h} Z`
  }

  return {
    type: 'path',
    x, y, width: w, height: h,
    fill:   parseAttr(el, 'fill', ''),
    stroke: parseAttr(el, 'stroke', ''),
    strokeWidth: parseFloat2(el.getAttribute('stroke-width'), 0),
    data: d,
    text: null, fontSize: 0, fontFamily: ''
  }
}

function parseText(el) {
  return {
    type: 'text',
    x: parseFloat2(el.getAttribute('x')),
    y: parseFloat2(el.getAttribute('y')),
    width: 0, height: 0,
    fill:   parseAttr(el, 'fill', '#ffffff'),
    stroke: '', strokeWidth: 0, data: '',
    text:       el.textContent?.trim() || '',
    fontSize:   parseFloat2(el.getAttribute('font-size'), 12),
    fontFamily: parseAttr(el, 'font-family', 'system-ui, sans-serif')
  }
}

/**
 * Parse a <circle> to a seat request.
 * Handles normal format (A_1), prefixed format (Slan_A_1), and Figma deduplications (A_1_2, Slan_A_1_2).
 */
function parseSeat(el) {
  const id    = el.getAttribute('id') || ''
  const parts = id.split('_')
  if (parts.length < 2) return null

  // Handle Figma dedup (e.g., A_1_2) where last two parts are numbers
  if (parts.length >= 3 && /^\d+$/.test(parts[parts.length - 1]) && /^\d+$/.test(parts[parts.length - 2])) {
    parts.pop()
  }

  const rowLabel = parts[parts.length - 2]
  const seatName = parts[parts.length - 1]
  const cx = parseFloat2(el.getAttribute('cx'))
  const cy = parseFloat2(el.getAttribute('cy'))
  const r  = parseFloat2(el.getAttribute('r'), 10)

  return {
    rowLabel,
    seatRequest: { seatName, svgElementId: id, x: cx, y: cy, radius: r }
  }
}

// ─── Row grouping ──────────────────────────────────────────────────────────────

function groupSeatsByRow(seats) {
  const rowMap = {}
  seats.forEach(({ rowLabel, seatRequest }) => {
    if (!rowMap[rowLabel]) rowMap[rowLabel] = []
    rowMap[rowLabel].push(seatRequest)
  })

  return Object.entries(rowMap)
    .sort(([a], [b]) => a.localeCompare(b))
    .map(([rowLabel, seatRequests]) => ({
      rowLabel,
      seatRequests: seatRequests.sort((a, b) => {
        const na = parseInt(a.seatName), nb = parseInt(b.seatName)
        if (!isNaN(na) && !isNaN(nb)) return na - nb
        return a.seatName.localeCompare(b.seatName)
      })
    }))
}

// ─── Zone group parser ────────────────────────────────────────────────────────

/**
 * Process a single child element inside a zone group.
 * Appends parsed seats and svgElements into the provided arrays.
 */
function processChild(child, seats, svgElements, warnings, zoneId) {
  const tag = child.tagName.toLowerCase()

  if (tag === 'circle') {
    const parsed = parseSeat(child)
    if (parsed) {
      seats.push(parsed)
    } else {
      warnings.push(
        `Circle "${child.getAttribute('id') || '(no id)'}" trong zone "${zoneId}" có ID không đúng định dạng. ` +
        `Cần dạng: RowLabel_SoGhe (ví dụ: A_1, B_12).`
      )
    }
  } else if (tag === 'path') {
    const fill   = child.getAttribute('fill')   || ''
    const stroke = child.getAttribute('stroke') || ''
    if (hasGradientFill(fill) || hasGradientFill(stroke)) {
      warnings.push(`Path trong zone "${zoneId}" dùng màu gradient — sẽ bị mất màu khi lưu. Hãy đổi sang Solid color.`)
    }
    svgElements.push(parsePath(child))
  } else if (tag === 'rect') {
    svgElements.push(parseRect(child))
  } else if (tag === 'text') {
    svgElements.push(parseText(child))
  } else if (tag === 'g') {
    // Nested group → flatten one level (covers Figma's sub-groups)
    Array.from(child.children).forEach(nested => processChild(nested, seats, svgElements, warnings, zoneId))
  }
  // Other tags (defs, use, image…) are silently ignored
}

/**
 * Parse a zone <g> element into the zone DTO.
 */
function parseZoneGroup(groupEl, displayOrder) {
  const id       = (groupEl.getAttribute('id') || '').trim()
  const children = Array.from(groupEl.children)

  const svgElements = []
  const seats       = []
  const warnings    = []

  children.forEach(child => processChild(child, seats, svgElements, warnings, id))

  const rows       = groupSeatsByRow(seats)
  const totalSeats = seats.length
  const isStage    = isStageZone(id)
  const isReservingSeat = totalSeats > 0
  const isSalable       = !isStage

  // Bounding box: use all children for accuracy
  const allChildrenFlat = []
  groupEl.querySelectorAll('circle, rect').forEach(el => allChildrenFlat.push(el))
  const bbox = allChildrenFlat.length > 0
    ? computeBoundingBox(allChildrenFlat)
    : computeBoundingBox(children)

  const zone = {
    zoneName:       id,
    color:          '#6366f1',  // user configures in step 3
    x: bbox.x, y: bbox.y, width: bbox.width, height: bbox.height,
    capacity:       totalSeats,
    isStage, isReservingSeat, isSalable,
    svgElementId:   id,
    basePrice:      0,
    displayOrder,
    svgElements,
    rows,
    _parsed: {
      totalSeats,
      rowCount:   rows.length,
      rowDetails: rows.map(r => ({ rowLabel: r.rowLabel, count: r.seatRequests.length }))
    }
  }

  return { zone, warnings }
}

// ─── Zone discovery ───────────────────────────────────────────────────────────

/**
 * Find all zone groups (<g id="Zone-*"> or <g id="STAGE">) in the SVG document,
 * regardless of nesting depth.
 *
 * Strategy:
 *  1. Collect all <g id="..."> elements in the entire document.
 *  2. Keep only those whose id matches the zone naming convention.
 *  3. Remove any that are themselves descendants of another matched zone
 *     (to avoid counting nested groups inside a zone as separate zones).
 */
function findZoneGroups(svgEl) {
  // All <g> elements with an id anywhere in the SVG
  const candidates = Array.from(svgEl.querySelectorAll('g[id]'))
    .filter(isZoneGroup)

  if (candidates.length === 0) return []

  // Remove descendants of other zone groups (keep only top-level zones)
  const topZones = candidates.filter(el => {
    let parent = el.parentElement
    while (parent && parent !== svgEl) {
      if (isZoneGroup(parent)) return false  // this is a child of another zone
      parent = parent.parentElement
    }
    return true
  })

  return topZones
}

// ─── Public API ───────────────────────────────────────────────────────────────

/**
 * Parse an SVG File object.
 * @param {File} file
 * @returns {Promise<{ parsed, svgWidth, svgHeight, svgText, warnings, errors }>}
 */
export async function parseSVGFile(file) {
  const text = await file.text()
  return parseSVGString(text)
}

/**
 * Parse an SVG string.
 * @param {string} svgText
 * @returns {{ parsed, svgWidth, svgHeight, svgText, warnings, errors }}
 */
export function parseSVGString(svgText) {
  const parser = new DOMParser()
  const doc    = parser.parseFromString(svgText, 'image/svg+xml')

  const parseError = doc.querySelector('parsererror')
  if (parseError) {
    return {
      parsed: null, svgWidth: 0, svgHeight: 0, warnings: [],
      errors: ['File SVG không hợp lệ: ' + (parseError.textContent?.slice(0, 120) ?? '')]
    }
  }

  const svgEl = doc.querySelector('svg')
  if (!svgEl) {
    return {
      parsed: null, svgWidth: 0, svgHeight: 0, warnings: [],
      errors: ['Không tìm thấy thẻ <svg> trong file.']
    }
  }

  // Dimensions — prefer explicit width/height, fall back to viewBox
  const vb = svgEl.getAttribute('viewBox')?.split(/[\s,]+/) ?? []
  const widthAttr = svgEl.getAttribute('width')
  const heightAttr = svgEl.getAttribute('height')
  
  const parsedWidth = widthAttr && !widthAttr.includes('%') ? parseFloat2(widthAttr, 0) : parseFloat2(vb[2], 0)
  const parsedHeight = heightAttr && !heightAttr.includes('%') ? parseFloat2(heightAttr, 0) : parseFloat2(vb[3], 0)

  const svgWidth  = parsedWidth
  const svgHeight = parsedHeight

  // Discover zone groups
  const zoneGroups = findZoneGroups(svgEl)

  if (zoneGroups.length === 0) {
    return {
      parsed: null, svgWidth, svgHeight, warnings: [],
      errors: [
        'SVG không chứa phân khu hợp lệ nào. ' +
        'Mỗi phân khu phải là thẻ <g> với id="STAGE" hoặc id bắt đầu bằng "Zone-" (ví dụ: <g id="Zone-VIP">).'
      ]
    }
  }

  const zones      = []
  const allWarnings = []

  zoneGroups.forEach((groupEl, index) => {
    const { zone, warnings } = parseZoneGroup(groupEl, index)
    zones.push(zone)
    allWarnings.push(...warnings)
  })

  return {
    parsed: { zones, svgWidth, svgHeight },
    svgWidth, svgHeight, svgText,
    warnings: allWarnings,
    errors: []
  }
}
