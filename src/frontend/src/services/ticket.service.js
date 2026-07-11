import api from './api/axios'
import { MY_TICKETS } from './api/endpoints'

export const ticketService = {
  /**
   * Fetch user's tickets
   * @param {Object} params - Query parameters
   * @param {number} [params.status] - Status filter (e.g. 1 for Valid, 2 for Used)
   * @param {number} [params.pageIndex=1] - Page number
   * @param {number} [params.pageSize=10] - Items per page
   * @returns {Promise<import('../stores/eventStore').PaginatedResult>}
   */
  async getMyTickets(params = {}) {
    const { status, pageIndex = 1, pageSize = 10 } = params;
    const query = new URLSearchParams();
    if (status) query.append('status', status);
    query.append('pageNumber', pageIndex);
    query.append('pageSize', pageSize);

    const response = await api.get(`${MY_TICKETS}?${query.toString()}`);
    return response.data;
  }
}
