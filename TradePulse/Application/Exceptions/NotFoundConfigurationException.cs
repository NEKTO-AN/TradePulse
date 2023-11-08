namespace Application.Exceptions
{
    public class NotFoundConfigurationException : Exception
    {
        private readonly string _message;
        public override string Message => _message;
        public NotFoundConfigurationException(string variableName)
        {
            _message = $"Configuration variable was not found. Variable name: {variableName}";
        }
    }
}