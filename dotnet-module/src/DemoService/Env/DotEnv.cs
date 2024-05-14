namespace DevEpos.CF.Demo.Env {
    /// <summary>
    /// Loads Environment variables from .env file into the current environment<br/>
    /// NOTE: Should only be used in 'dev' environment
    /// </summary>
    public static class DotEnv {
        public static void Load(string filePath) {
            if (!File.Exists(filePath)) {
                return;
            }

            foreach (var line in File.ReadAllLines(filePath)) {
                var firstEqualChar = line.IndexOf("=");
                if (firstEqualChar != -1) {
                    var variableName = line.Substring(0, firstEqualChar);
                    var variableValue = line.Substring(
                        firstEqualChar + 1,
                        line.Length - firstEqualChar - 1
                    );

                    if (variableName != null && variableValue != null) {
                        Environment.SetEnvironmentVariable(
                            variableName.Trim(),
                            variableValue.Trim()
                        );
                    }
                }
            }
        }
    }
}
