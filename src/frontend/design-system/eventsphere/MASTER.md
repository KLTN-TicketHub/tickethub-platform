# Design System Master File (Emerald Edition)

> **LOGIC:** When building a specific page, first check `design-system/pages/[page-name].md`.
> If that file exists, its rules **override** this Master file.
> If not, strictly follow the rules below.

---

**Project:** TicketHub
**Version:** 2.0 (Modernized)
**Category:** Premium Ticketing Platform

---

## Global Design Tokens

### Color Palette (Dark Mode First)

| Role | Component | Status | Token usage | Premium Polish |
| :--- | :--- | :--- | :--- | :--- |
| **BaseTable** | 🟢 Done | `bg-card`, `border-main` | Hover Glow, Variant support |
| **Admin Dashboard** | 🟢 Done | `text-main`, `text-muted` | Vibrant Stats, Smooth Anim |
| **Events Admin** | 🟢 Done | `bg-surface`, `border-main` | Filter bars, Quick actions |

| Role | Hex | Tailwind Token | Usage |
|------|-----|----------------|-------|
| **Primary** | `#00C853` | `text-primary`, `bg-primary` | Main CTA, Branding, Highlights |
| **Main Text** | `#E2F0E6` | `text-main` | Headings, Primary Body Text |
| **Muted Text** | `#6A8A72` | `text-muted` | Subtitles, Captions, Placeholders |
| **Background** | `#0A0F0D` | `bg-bg` | Page Background |
| **Surface** | `#111916` | `bg-surface` | Section containers, Modals |
| **Card** | `#141B16` | `bg-card` | Event cards, Input fields |
| **Border** | `#1C2B20` | `border-border-main` | Global borders, Dividers |
| **Danger** | `#FF4757` | `text-danger`, `bg-danger` | Errors, Destructive actions |

### Typography

- **Heading Font:** "Clash Display", sans-serif (Bold, Elegant)
- **Body Font:** "DM Sans", sans-serif (Readable, Modern)
- **Mono Font:** "JetBrains Mono", monospace (Tickets, Codes)

**CSS Import:**
```css
@import url('https://fonts.googleapis.com/css2?family=Clash+Display:wght@400;500;600;700&family=DM+Sans:ital,wght@0,300;0,400;0,500;0,600;1,400&family=JetBrains+Mono:wght@400;500&display=swap');
```

---

## Semantic Token Architecture

**CRITICAL RULE:** Do NOT use arbitrary Tailwind colors (e.g., `text-blue-500`). Use semantic tokens only.

| Class | CSS Variable | Purpose |
|-------|--------------|---------|
| `text-main` | `var(--color-main)` | High-contrast text on dark backgrounds |
| `text-muted` | `var(--color-muted)` | Low-contrast text for secondary info |
| `bg-surface` | `var(--color-surface)` | Level 1 elevation (lighter than bg) |
| `bg-card` | `var(--color-card)` | Level 2 elevation (lighter than surface) |
| `border-border-main` | `var(--color-border-main)` | Standard border for all components |

---

## Component Specs

### Buttons

Use `BaseButton.vue`. Variants:
- `primary`: Background Emerald, Text Black.
- `secondary`: Transparent, Border Emerald, Text Emerald.
- `ghost`: Transparent, Text Muted, Hover Main.

### Cards

Use `EventCard.vue`.
- Border: `border-border-main`
- Background: `bg-card`
- Hover: `-translate-y-1.5`, `border-primary`, custom emerald shadow.

### Inputs

Use `BaseInput.vue`.
- Background: `bg-card`
- Border: `border-border-main`
- Focus: `border-primary`, `outline-none`.

---

## Style Guidelines

**Mood:** Premium, Energetic, Tech-forward.

### Do's (REQUIRED)
- ✅ Use **glassmorphism** effects (backdrop-blur) for overlays.
- ✅ Add **subtle transitions** (300ms) to all hover states.
- ✅ Use **Emerald Green** (`#00C853`) for all primary actions.
- ✅ Ensure all clickable elements have `cursor-pointer`.

### Anti-Patterns (FORBIDDEN)
- ❌ **Legacy Tokens:** Never use `text-text-main` or `text-text-muted`.
- ❌ **Arbitrary Values:** Avoid `p-[13px]`, use standard spacing if possible.
- ❌ **Default Colors:** Do not use `text-white` directly for body text; use `text-main`.
- ❌ **Layout Shifts:** Avoid scaling that shifts adjacent elements.

---

## Delivery Checklist

- [x] All legacy `text-text-*` tokens removed.
- [x] Emerald Green theme applied globally.
- [x] Dark mode accessibility (contrast) verified.
- [x] Mobile responsiveness (375px - 1440px) checked.
- [x] Hover states and transitions applied.
- [x] Semantic Token Architecture strictly followed.
orizontal scroll on mobile
