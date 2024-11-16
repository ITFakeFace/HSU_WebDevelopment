namespace LMS_ServerAPI.Models.APIHelpers
{
    public class ResponseHandler
    {
        public int Code { get; set; }
        public bool Status { get; set; }
        public String? Message { get; set; }
        public dynamic? Data { get; set; }
    }
}
