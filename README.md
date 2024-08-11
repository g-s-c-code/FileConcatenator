# FileConcatenator

## Overview

**FileConcatenator** is a utility designed to streamline the process of concatenating multiple code or text files into a single output. This tool is particularly useful for developers who need to quickly gather code from various files for analysis, review, or input into large language models like GPT. While the primary use case is to gather and concatenate code files efficiently, the application is versatile and can be tailored to handle a wide range of file types and scenarios.

## Preview
<img src="https://raw.githubusercontent.com/g-s-c-code/FileConcatenator/master/fileconcatenator.png" />

## Features

- **Effortless File Concatenation**: Merge files in a directory into a single output, with filtering by type (e.g., `*.cs`, `*.txt`).
- **Clipboard Integration**: Automatically copies the result to the clipboard for easy sharing or pasting.
- **Customizable Settings**:
  - **Base Directory**: Change the source directory for files.
  - **Clipboard Limit**: Set the maximum characters for clipboard content.
  - **File Type Selection**: Choose file types for concatenation.
  - **Hidden Files**: Toggle inclusion of hidden files.
- **Persistent Configuration**: Settings are saved between sessions for a consistent experience.
- **User-Friendly Interface**: Command-line interface with Spectre.Console for easy navigation and command execution.

## Installation

To install **FileConcatenator**, ensure .NET 8.0 SDK is installed. Clone the repository, build, and run:

```bash
git clone https://github.com/g-s-c-code/FileConcatenator.git
cd FileConcatenator
dotnet build
dotnet run
```

## Creating Executables

For win-x64 based systems you can simply [download the .exe](https://github.com/g-s-c-code/FileConcatenator/blob/master/FileConcatenator.exe).

To create a runnable file for different OSes, run the following:

- **Windows**: 
  ```bash
  dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
  ```
- **Linux (ARM64)**:
  ```bash
  dotnet publish -c Release -r linux-arm64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
  ```
- **Linux (x64)**:
  ```bash
  dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
  ```
- **macOS (ARM64)**:
  ```bash
  dotnet publish -c Release -r osx-arm64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
  ```
- **macOS (x64)**:
  ```bash
  dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
  ```

## Usage

Commands include:

- **`cd <directory>`**: Change directory.
- **`1` Concatenate & Copy to Clipboard**: Merge files and copy to clipboard.
- **`2` Set Clipboard Limit**: Adjust clipboard character limit.
- **`3` Set File Types**: Choose file types for concatenation.
- **`4` Set Base Path (enter manually)**: Set directory manually.
- **`5` Set Base Path to Current Directory**: Use the current directory.
- **`6` Show Hidden Files**: Toggle hidden file visibility.
- **`7` Change Theme**: Modify the UI color scheme.
- **`H` Help**: Show help information.
- **`Q` Quit**: Exit the application.

## Configuration

Configuration is stored in `settings.json` and includes:

- **BaseDirectoryPath**: Directory for file operations.
- **ClipboardCharacterLimit**: Max characters in clipboard.
- **FileTypes**: List of file types to include.
- **ShowHiddenFiles**: Include hidden files.

Edit `settings.json` manually or adjust via the application.

## Troubleshooting

- **Clipboard Issues**: Reduce clipboard limit if problems occur.
- **File Access**: Ensure permissions for files and directories.
- **Directory Structure**: Use `cd` to navigate directories correctly.

For issues or suggestions, contribute on the [GitHub repository](https://github.com/g-s-c-code/FileConcatenator).


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
