namespace LibraryManagementSystem.Helper
{
    public class ResponseHandler<T> where T : class
    {
        public String StatusCode { get; set; }
        public String Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
