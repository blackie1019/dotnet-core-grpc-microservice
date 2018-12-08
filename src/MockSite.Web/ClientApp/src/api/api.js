import axios from 'axios';

export async function sendClientErrorToServer(error) {
  return await axios.post(`https://localhost:5001/api/ErrorLog`, error);
}

export async function getData() {
  return await axios.post(`https://localhost:5001/api/getData`);
}
