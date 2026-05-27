const http = require('http');

async function request(method, path, body = null, token = null) {
    return new Promise((resolve, reject) => {
        const options = {
            hostname: 'localhost',
            port: 5219, // Use 5219 instead of 5296
            path: path,
            method: method,
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if (token) {
            options.headers['Authorization'] = `Bearer ${token}`;
        }

        const req = http.request(options, (res) => {
            let data = '';
            res.on('data', chunk => data += chunk);
            res.on('end', () => {
                resolve({
                    status: res.statusCode,
                    data: data ? JSON.parse(data) : null
                });
            });
        });

        req.on('error', reject);

        if (body) {
            req.write(JSON.stringify(body));
        }
        req.end();
    });
}

async function runTests() {
    console.log("=== BẮT ĐẦU TEST TOÀN BỘ API ===");
    try {
        // 1. Đăng ký testuser1
        console.log("1. Đăng ký testuser1...");
        let res = await request('POST', '/api/Auth/register', {
            username: "testuser_test",
            password: "Password@123",
            fullName: "Test User",
            phone: "0123456789"
        });
        console.log("-> Register Status:", res.status); // Expect 200 or 400 if exists

        // 2. Đăng nhập Admin
        console.log("\n2. Đăng nhập Admin...");
        res = await request('POST', '/api/Auth/login', {
            username: "admin",
            password: "Admin@123"
        });
        console.log("-> Admin Login Status:", res.status);
        if (res.status !== 200) throw new Error("Admin login failed!");
        const adminToken = res.data.token;

        // 3. Admin: Lấy danh sách Users
        console.log("\n3. Lấy danh sách Users...");
        res = await request('GET', '/api/Users', null, adminToken);
        console.log("-> Get Users Status:", res.status);
        const testUser = res.data.find(u => u.username === "testuser_test");
        if (!testUser) throw new Error("Không tìm thấy testuser_test!");
        const testUserId = testUser.id;

        // 4. Admin: Khóa testuser
        console.log(`\n4. Admin: Khóa user ID ${testUserId}...`);
        res = await request('PUT', `/api/Users/${testUserId}/lock`, null, adminToken);
        console.log("-> Lock User Status:", res.status);

        // 5. User: Đăng nhập testuser bị khóa
        console.log("\n5. User: Đăng nhập khi bị khóa...");
        res = await request('POST', '/api/Auth/login', {
            username: "testuser_test",
            password: "Password@123"
        });
        console.log("-> Login Locked User Status (Expect 401):", res.status);

        // 6. Admin: Mở khóa testuser
        console.log(`\n6. Admin: Mở khóa user ID ${testUserId}...`);
        res = await request('PUT', `/api/Users/${testUserId}/lock`, null, adminToken);
        console.log("-> Unlock User Status:", res.status);

        // 7. User: Đăng nhập testuser sau khi mở khóa
        console.log("\n7. User: Đăng nhập sau khi mở khóa...");
        res = await request('POST', '/api/Auth/login', {
            username: "testuser_test",
            password: "Password@123"
        });
        console.log("-> Login Unlocked User Status:", res.status);
        const userToken = res.data.token;

        // 8. User: Cập nhật Profile
        console.log("\n8. User: Cập nhật Profile...");
        res = await request('PUT', '/api/Auth/profile', {
            fullName: "Test User Updated",
            phone: "0999888777"
        }, userToken);
        console.log("-> Update Profile Status:", res.status);

        // 9. User: Tạo Kèo mới
        console.log("\n9. User: Tạo Kèo mới...");
        let tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const matchDate = tomorrow.toISOString().split('T')[0];
        
        res = await request('POST', '/api/Matches', {
            courtId: 1,
            date: matchDate,
            timeStart: "18:00",
            timeEnd: "20:00",
            level: 1,
            slotsTotal: 4,
            slotsFilled: 1,
            cost: 50000,
            note: "Kèo test auto",
            zalo: "0123456789"
        }, userToken);
        console.log("-> Create Match Status:", res.status);
        if (res.status !== 200 && res.status !== 201) console.error("-> Body:", res.data);
        const matchId = res.data?.id;

        // 10. User/Public: Tìm kiếm Kèo theo ngày
        console.log(`\n10. Tìm kiếm Kèo theo ngày ${matchDate}...`);
        res = await request('GET', `/api/Matches?date=${matchDate}`);
        console.log("-> Filter Matches Status:", res.status);
        const foundMatches = res.data.items || res.data;
        console.log(`-> Found ${foundMatches.length} matches for that date.`);

        // 11. User: Gửi Feedback
        console.log("\n11. User: Gửi Góp ý...");
        res = await request('POST', '/api/Feedback', {
            isHelpful: true,
            missingFeature: "Cần thêm chatbot",
            wantedCourt: "Sân test 999 Biên Hòa"
        }, userToken);
        console.log("-> Create Feedback Status:", res.status);

        // 12. Admin: Xem Góp ý
        console.log("\n12. Admin: Xem Góp ý...");
        res = await request('GET', '/api/Feedback', null, adminToken);
        console.log("-> Get Feedback Status:", res.status);
        console.log("-> Total Feedbacks:", res.data.length);

        // 13. Admin: Dashboard Stats (Biểu đồ)
        console.log("\n13. Admin: Dashboard Stats...");
        res = await request('GET', '/api/Stats/dashboard', null, adminToken);
        console.log("-> Get Dashboard Status:", res.status);
        console.log("-> MatchCountsByDate length:", res.data.matchCountsByDate.length);

        console.log("\n=== TẤT CẢ TEST ĐÃ HOÀN TẤT ===");
    } catch (e) {
        console.error("TEST FAILED:", e);
    }
}

runTests();
