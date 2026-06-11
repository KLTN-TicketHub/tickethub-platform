/**
 * svgValidator.js
 * 
 * Validates the parsed seatmap DTO before submitting to the backend.
 * Returns { valid: boolean, errors: string[], warnings: string[] }
 */

const SEAT_ID_PATTERN = /^[A-Za-z0-9]+(_.+){2,}$/  // ZonePrefix_RowLabel_SeatNum at minimum

/**
 * Check if a fill color uses gradient syntax
 */
function isGradientFill(val) {
  return typeof val === 'string' && val.trim().startsWith('url(')
}

/**
 * Validate seatMapName
 */
function validateSeatMapName(name, errors) {
  if (!name || !name.trim()) {
    errors.push('Tên sơ đồ ghế không được để trống.')
  } else if (name.trim().length < 3) {
    errors.push('Tên sơ đồ ghế phải có ít nhất 3 ký tự.')
  } else if (name.trim().length > 200) {
    errors.push('Tên sơ đồ ghế không được vượt quá 200 ký tự.')
  }
}

/**
 * Validate SVG dimensions
 */
function validateDimensions(svgWidth, svgHeight, errors) {
  if (!svgWidth || svgWidth <= 0) {
    errors.push('SVG phải có thuộc tính width hợp lệ > 0.')
  }
  if (!svgHeight || svgHeight <= 0) {
    errors.push('SVG phải có thuộc tính height hợp lệ > 0.')
  }
}

/**
 * Validate zones array
 */
function validateZones(zones, errors, warnings) {
  if (!zones || zones.length === 0) {
    errors.push('Sơ đồ ghế phải có ít nhất 1 phân khu (zone).')
    return
  }

  const zoneIds = new Set()
  const allSeatIds = new Set()

  zones.forEach((zone, zoneIdx) => {
    const zLabel = `Zone[${zoneIdx}] "${zone.zoneName}"`

    // Zone ID / name validation
    if (!zone.zoneName || !zone.zoneName.trim()) {
      errors.push(`${zLabel}: zoneName không được để trống.`)
      return
    }

    if (!zone.svgElementId) {
      errors.push(`${zLabel}: thiếu svgElementId.`)
    }

    // Duplicate zone name check
    if (zoneIds.has(zone.zoneName)) {
      errors.push(`Zone "${zone.zoneName}" bị trùng lặp. Mỗi zone phải có tên duy nhất.`)
    }
    zoneIds.add(zone.zoneName)

    // Zone naming convention
    if (!zone.isStage && !zone.zoneName.startsWith('Zone-') && zone.zoneName !== 'STAGE') {
      warnings.push(`${zLabel}: Tên zone không bắt đầu bằng "Zone-". Theo quy ước, các zone ghế phải có tên dạng "Zone-VIP", "Zone-Standard"...`)
    }

    // Stage zone must not have seats
    if (zone.isStage && zone.rows && zone.rows.length > 0) {
      errors.push(`${zLabel}: Khu sân khấu (STAGE) không được chứa ghế ngồi.`)
    }

    // Salable zone config
    if (zone.isSalable) {
      if (zone.basePrice < 0) {
        errors.push(`${zLabel}: Giá bán (basePrice) không được âm.`)
      }
      if (zone.displayOrder === undefined || zone.displayOrder === null || zone.displayOrder < 0) {
        errors.push(`${zLabel}: Thứ tự hiển thị (displayOrder) phải >= 0.`)
      }
      if (!zone.color || !zone.color.trim()) {
        errors.push(`${zLabel}: Màu sắc (color) không được để trống.`)
      }
    }

    // Non-reserving salable zones (GA) should have no rows
    if (zone.isSalable && !zone.isReservingSeat) {
      if (zone.rows && zone.rows.length > 0) {
        errors.push(`${zLabel}: Khu vực đứng (không có ghế cố định) không được có danh sách rows.`)
      }
      if (zone.capacity <= 0) {
        errors.push(`${zLabel}: Khu vực đứng (GA) phải có capacity > 0.`)
      }
    }

    // Reserved seating zone must have rows
    if (zone.isSalable && zone.isReservingSeat) {
      if (!zone.rows || zone.rows.length === 0) {
        errors.push(`${zLabel}: Khu vực ghế ngồi cố định phải có ít nhất 1 hàng (row).`)
      }
    }

    // Validate rows and seats
    if (zone.rows) {
      const rowLabels = new Set()
      zone.rows.forEach((row, rowIdx) => {
        const rLabel = `${zLabel} → Hàng "${row.rowLabel}"`

        if (!row.rowLabel || !row.rowLabel.trim()) {
          errors.push(`${rLabel}: rowLabel không được rỗng.`)
        }

        if (rowLabels.has(row.rowLabel)) {
          errors.push(`${zLabel}: Hàng "${row.rowLabel}" bị trùng lặp. Mỗi hàng phải có ký hiệu duy nhất trong zone.`)
        }
        rowLabels.add(row.rowLabel)

        if (!row.seatRequests || row.seatRequests.length === 0) {
          errors.push(`${rLabel}: Hàng phải có ít nhất 1 ghế.`)
        }

        if (row.seatRequests) {
          row.seatRequests.forEach((seat, seatIdx) => {
            const sLabel = `${rLabel} → Ghế "${seat.svgElementId}"`

            // Validate seat ID format
            if (!seat.svgElementId) {
              errors.push(`${rLabel}: Ghế[${seatIdx}] thiếu svgElementId.`)
            } else {
              // Check pattern: must have at least 2 parts (RowLabel_SeatNumber)
              const parts = seat.svgElementId.split('_')
              if (parts.length < 2) {
                errors.push(`${sLabel}: ID ghế không đúng định dạng. Cần dạng: RowLabel_SoGhe (ví dụ: A_1, B_12).`)
              }

              // Unique seat ID across all zones
              if (allSeatIds.has(seat.svgElementId)) {
                errors.push(`ID ghế "${seat.svgElementId}" bị trùng lặp. Mỗi ghế phải có ID duy nhất trên toàn sơ đồ.`)
              }
              allSeatIds.add(seat.svgElementId)
            }

            if (!seat.seatName || !seat.seatName.trim()) {
              errors.push(`${sLabel}: seatName không được rỗng.`)
            }

            if (seat.x === undefined || seat.x === null || isNaN(seat.x)) {
              errors.push(`${sLabel}: Tọa độ x không hợp lệ.`)
            }
            if (seat.y === undefined || seat.y === null || isNaN(seat.y)) {
              errors.push(`${sLabel}: Tọa độ y không hợp lệ.`)
            }
            if (!seat.radius || seat.radius <= 0) {
              errors.push(`${sLabel}: Bán kính (radius) phải > 0.`)
            }
          })
        }
      })
    }

    // Validate svgElements
    if (zone.svgElements) {
      zone.svgElements.forEach((el, elIdx) => {
        if (!el.type || !['path', 'text', 'circle', 'rect', 'ellipse'].includes(el.type)) {
          warnings.push(`${zLabel}: svgElement[${elIdx}] có type không rõ: "${el.type}". Có thể không hiển thị đúng.`)
        }
        if (isGradientFill(el.fill)) {
          errors.push(`${zLabel}: svgElement[${elIdx}] sử dụng màu gradient (url(#...)). Gradient không được hỗ trợ — hãy đổi sang màu solid.`)
        }
        if (el.type === 'path' && (!el.data || !el.data.trim())) {
          warnings.push(`${zLabel}: svgElement[${elIdx}] là path nhưng không có dữ liệu d="...".`)
        }
        if (el.type === 'text' && (!el.text || !el.text.trim())) {
          warnings.push(`${zLabel}: svgElement[${elIdx}] là text nhưng không có nội dung.`)
        }
      })
    }
  })
}

/**
 * Main validation function.
 * @param {object} params - { seatMapName, svgWidth, svgHeight, zones }
 * @returns {{ valid: boolean, errors: string[], warnings: string[] }}
 */
export function validateSeatMap({ seatMapName, svgWidth, svgHeight, zones }) {
  const errors = []
  const warnings = []

  validateSeatMapName(seatMapName, errors)
  validateDimensions(svgWidth, svgHeight, errors)
  validateZones(zones, errors, warnings)

  return {
    valid: errors.length === 0,
    errors,
    warnings
  }
}

/**
 * Validates zone-level config set by user (basePrice, displayOrder, color).
 */
export function validateZoneConfig(zones) {
  const errors = []
  zones.forEach((zone, idx) => {
    if (!zone.isSalable) return
    if (zone.basePrice < 0) {
      errors.push(`Zone "${zone.zoneName}": Giá bán không được âm.`)
    }
    if (!zone.color || !zone.color.match(/^#[0-9A-Fa-f]{3,8}$|^rgba?\(/)) {
      errors.push(`Zone "${zone.zoneName}": Màu sắc không hợp lệ (phải là mã hex #RRGGBB hoặc rgba(...)).`)
    }
    if (isNaN(parseInt(zone.displayOrder)) || parseInt(zone.displayOrder) < 0) {
      errors.push(`Zone "${zone.zoneName}": Thứ tự hiển thị (displayOrder) phải là số nguyên >= 0.`)
    }
  })
  return { valid: errors.length === 0, errors }
}
