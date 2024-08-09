# FileConcatenator

## Overview

**FileConcatenator** is a utility designed to streamline the process of concatenating multiple code or text files into a single output. This tool is particularly useful for developers who need to quickly gather code from various files for analysis, review, or input into large language models like GPT. While the primary use case is to gather and concatenate code files efficiently, the application is versatile and can be tailored to handle a wide range of file types and scenarios.

## Features

- **Concatenate Files with Ease**: Combine the contents of multiple files in a directory into a single output, with the ability to filter by file types (e.g., `*.cs`, `*.txt`).
- **Clipboard Integration**: Automatically copies the concatenated content to your clipboard, enabling quick sharing or pasting into other applications.
- **Customizable Settings**:
  - **Base Directory**: Set and switch the directory from which files are gathered.
  - **Clipboard Character Limit**: Define the maximum number of characters that can be copied to the clipboard.
  - **File Type Selection**: Choose which file types to include in the concatenation process.
  - **Hidden Files Visibility**: Toggle the inclusion of hidden files.
- **Persistent Configuration**: All settings are saved between sessions, ensuring a seamless experience each time you run the application.
- **User-Friendly Interface**: A clear command-line interface powered by Spectre.Console, making it easy to navigate directories, adjust settings, and execute commands.

## Installation

To install and run **FileConcatenator**, ensure you have the .NET 8.0 SDK installed on your system. You can clone the repository and build the project:

```bash
git clone https://github.com/g-s-c-code/FileConcatenator.git
cd FileConcatenator
dotnet build
dotnet run
```

## Usage

After running the application, you will be presented with a command-line interface where you can execute the following commands:

### Main Commands

- **`cd <directory>`**: Change the current working directory.
- **`1` Concatenate & Copy to Clipboard**: Concatenates the files in the current directory (based on the specified file types) and copies the result to the clipboard.
- **`2` Set Clipboard Limit**: Adjust the maximum number of characters to copy to the clipboard.
- **`3` Set File Types**: Specify the file types to target during concatenation (e.g., `*.cs, *.txt`).
- **`4` Set Base Path (enter manually)**: Manually set the base directory for future file operations.
- **`5` Set Base Path to Current Directory**: Quickly sets the base path to the current directory.
- **`6` Show Hidden Files**: Toggle the visibility of hidden files in the directory listing.
- **`7` Change Theme**: Change the UI color scheme.
- **`H` Help**: Displays detailed help information.
- **`Q` Quit**: Exits the application.

### Examples

- **Change Directory**: To navigate to a specific folder, use the command:
  ```bash
  cd /path/to/your/folder
  ```

- **Concatenate and Copy**: After setting the desired directory and file types, simply press `1` to concatenate the files and copy the result to your clipboard.

- **Set File Types**: To focus on specific file types, like `.cs` and `.txt`, use:
  ```bash
  3
  ```
  Then select the file types from the list presented.

## Configuration Management

The application maintains its configuration in a `settings.json` file, automatically created in the base directory. This configuration includes:

- **BaseDirectoryPath**: The root directory for file operations.
- **ClipboardCharacterLimit**: Maximum characters allowed in the clipboard.
- **FileTypes**: Comma-separated list of file extensions to target.
- **ShowHiddenFiles**: Boolean value to determine if hidden files are included.

You can manually edit the `settings.json` file if needed, or adjust settings through the application's interface.

## Troubleshooting & Tips

- **Clipboard Issues**: If the clipboard limit is too high, some systems may have difficulty handling the data. If you encounter problems, try reducing the limit.
- **File Access**: Ensure you have the necessary permissions to access all files and directories within the base path. Unauthorized access will result in skipped files.
- **Directory Structure**: Use the `cd` command to navigate to the correct directory before concatenating files, especially if working in nested directory structures.

If you encounter any issues or have suggestions for improvements, feel free to contribute to the project or open an issue on the [GitHub repository](https://github.com/g-s-c-code/FileConcatenator).
