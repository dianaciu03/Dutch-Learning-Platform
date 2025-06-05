import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    scenarios: {
        get_exams: {
            executor: 'constant-arrival-rate',
            rate: 60, // Reduced from 70 to 50 GET requests per second
            timeUnit: '1s',
            duration: '4m',
            preAllocatedVUs: 50, // Increased from 20
            maxVUs: 50, // Increased from 50
            exec: 'getExams',
        },
        post_exams: {
            executor: 'constant-arrival-rate',
            rate: 10, // Reduced from 30 to 20 POST requests per second
            timeUnit: '1s',
            duration: '4m',
            preAllocatedVUs: 50, // Increased from 20
            maxVUs: 50, // Increased from 50
            exec: 'postExams',
        },
    },
    thresholds: {
    checks: ['rate>0.95'],
}

};

const BASE_URL = 'http://127.0.0.1/exams';

// GET request function
export function getExams() {
    const res = http.get(BASE_URL);

    const success = check(res, {
        'GET /exams status is 200': (r) => r.status === 200,
    });

    if (!success) {
        console.error(`GET failed. Status: ${res.status}, Body: ${res.body}`);
    }

    sleep(1);
}

// POST request function
export function postExams() {
    const payload = JSON.stringify({
        name: "string",
        level: "B2",
        maxPoints: 100
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(BASE_URL, payload, params);

    const success = check(res, {
        'POST /exams status is 200': (r) => r.status === 200,
    });

    if (!success) {
        console.error(`POST failed. Status: ${res.status}, Body: ${res.body}`);
    }

    sleep(1);
}