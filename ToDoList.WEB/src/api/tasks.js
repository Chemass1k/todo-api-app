import axios from './axios';

export const getAllTasks = async () => {
    const token = localStorage.getItem('token');
    const response = await axios.get('/Tasks', {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}