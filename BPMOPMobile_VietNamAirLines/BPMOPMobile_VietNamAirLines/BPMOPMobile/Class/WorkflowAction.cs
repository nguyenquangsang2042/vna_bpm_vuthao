
using System.ComponentModel;

namespace BPMOPMobile.Class
{
    public class WorkflowAction
    {
        public enum Action
        {
            [Description("Duyệt")]
            Next = 1,

            [Description("Phê duyệt")]
            Approve = 2,

            [Description("Chuyển xử lý")]
            Forward = 3,

            [Description("Yêu cầu hiệu chỉnh")]
            Return = 4,

            [Description("Từ chối")]
            Reject = 5,

            [Description("Thu hồi")]
            Recall = 6,

            [Description("Yêu cầu bổ sung")]
            RequestInformation = 7,

            [Description("Thu hồi đã phê duyệt")]
            RecallAfterApproved = 8,

            [Description("Xin ý kiến tham vấn")]
            RequestIdea = 9,

            [Description("Cho ý kiến")]
            Idea = 10,

            [Description("Lưu")]
            Save = 11,

            [Description("Gửi")]
            Submit = 12,

            [Description("Đăng nhập")]
            Login = 13,

            [Description("Chia sẻ")]
            Share = 14,

            [Description("Bổ sung")]
            Additional = 15,

            [Description("Xem")]
            View = 16,

            [Description("Tải xuống")]
            Download = 17,

            [Description("Tạo mới")]
            Add = 18,

            [Description("Cập nhật")]
            Update = 19,

            [Description("Xóa")]
            Delete = 20,

            [Description("Tìm kiếm")]
            Search = 21,

            [Description("Hủy")]
            Cancel = 51,

            [Description("Tạo Task")]
            CreateTask = 54
        }
    }
}
