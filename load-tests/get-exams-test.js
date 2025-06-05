import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 50, // 50 virtual users
    duration: '2m', // test duration 2 minutes
};

const BASE_URL = 'http://127.0.0.1/exams';

export default function () {
    // GET request to fetch all exams
    const res = http.get(BASE_URL);

    check(res, {
        'GET /exams status is 200': (r) => r.status === 200,
    });

    // Small sleep to avoid spamming too aggressively
    sleep(2);
}
