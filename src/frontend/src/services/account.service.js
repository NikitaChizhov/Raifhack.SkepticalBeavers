import axios from 'axios';
import authHeader from './auth-header';

const API_URL = 'http://localhost:5000/';

class AccountService {
  getRestaurants() {
    return axios.get(API_URL + 'restaurants', { headers: authHeader() });
  }

  getRestaurant(id) {
    return axios.get(API_URL + `restaurants/${id}`, { headers: authHeader() });
  }
}

export default new AccountService();