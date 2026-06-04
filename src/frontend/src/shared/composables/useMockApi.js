/**
 * useMockApi — Network latency simulation utility.
 *
 * Every mock API call in the system should use `delay()` to realistically
 * simulate async network behaviour — loading states, race conditions,
 * and UX responsiveness are all tested this way.
 *
 * @example
 *   import { delay } from '@/shared/composables/useMockApi'
 *   await delay(800) // simulates 800ms latency
 */

/**
 * Returns a promise that resolves after `ms` milliseconds.
 * @param {number} ms — Delay duration in milliseconds (default 600).
 * @returns {Promise<void>}
 */
export function delay(ms = 600) {
  return new Promise((resolve) => setTimeout(resolve, ms))
}

/**
 * Wraps a value in a simulated API response after a delay.
 * Useful for one-liners where you want to return mock data.
 *
 * @template T
 * @param {T} data — The mock data to return.
 * @param {number} ms — Delay duration in milliseconds (default 600).
 * @returns {Promise<T>}
 *
 * @example
 *   const events = await mockResponse(mockEventsArray, 1000)
 */
export function mockResponse(data, ms = 600) {
  return new Promise((resolve) => setTimeout(() => resolve(data), ms))
}

/**
 * Simulates a failed API call after a delay.
 *
 * @param {string} message — Error message.
 * @param {number} ms — Delay duration in milliseconds (default 400).
 * @returns {Promise<never>}
 *
 * @example
 *   await mockError('Network error', 500)
 */
export function mockError(message = 'Something went wrong', ms = 400) {
  return new Promise((_, reject) =>
    setTimeout(() => reject(new Error(message)), ms),
  )
}
