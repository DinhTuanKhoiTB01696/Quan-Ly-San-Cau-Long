namespace BadmintonApp.Domain.Enums;

public enum ReportReason
{
    FakePhone = 1, // Số Zalo không tồn tại
    Spam = 2,      // Kèo giả/spam
    WrongInfo = 3  // Thông tin sai
}
