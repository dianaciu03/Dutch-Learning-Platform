import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 25, // Number of virtual users
    duration: '1m', // Duration of the test
    thresholds: {
        checks: ['rate>0.95'], // 95% of checks must pass
    },
};

const BASE_URL = 'http://127.0.0.1/exams';

export default function () {
    const payload = JSON.stringify({
        name: "string",
        level: "B2",
        maxPoints: 100
    });

    const params = {
        headers: {
            'Content-Type': 'application/json', // Important!
        },
    };

    const res = http.post(BASE_URL, payload, params);

    const success = check(res, {
        'status is 200': (r) => r.status === 200,
    });

    if (!success) {
        console.error(`Request failed. Status: ${res.status}, Body: ${res.body}`);
    }

    sleep(2);
}
