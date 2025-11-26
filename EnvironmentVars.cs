using System;

public static class EnvironmentVars
{
    public static string PostgreSlqContext
    {
        get
        {
            return Environment.GetEnvironmentVariable("POSTGRESQL_CONTEXT");
        }
    }
}
