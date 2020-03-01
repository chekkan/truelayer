namespace Api.TrueLayer
{
    public class CallbackBodyDto
    {
        public string Code { get; set; }

        // State contains the user id the code is for
        public string State { get; set; }
    }
}