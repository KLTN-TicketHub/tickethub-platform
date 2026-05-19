export class BaseService {
  constructor(api) {
    this.api = api
  }

  parseResponse(resp) {
    return resp && resp.data
  }
}
