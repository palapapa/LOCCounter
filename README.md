# LOCCounter
A simple tool that counts LOCs.
<br>
Command format:
```
    Format: <paths>; <extensions>; <recursive>; <exclude-extension>
    paths: Pipe-separated list of targeted paths. Place "?" in front to exclude.
    extensions: Comma-separated list of targeted file extensions. Leave blank for all extensions.
    recursive: "true" or "false". Whether to recursively search for files in the specified paths.
    exclude-extension: "true" or "false". Whether to exclude the extensions specified in "extensions" and search for all other extensions.
    Example: C:\Code | D:\Programming | ?D:\Programming\Secret; .cs, .cpp, .c; true; false
```