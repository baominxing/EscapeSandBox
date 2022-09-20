namespace EscapeSandBox.Api.Dto
{
    public class ResponseDto
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public object Data { get; set; }
    }
}
