A simple browser for Azure Devlops Builds and Artifacts.

## Features

* allows browsing azure builds using tags
* allows browsing azure build artifacts and their content
* supports in-browser artifact content rendering
* supports permalinks

## Usage

Update `appsettings.json` and set:
```
"DevopsClient": {
  "PersonalAccessToken": "[PERSONAL ACCESS TOKEN]",
  "ProjectUri": "https://dev.azure.com/[ORGANIZATION]/[PROJECT]"
} 
```

Compile and run.

## Supported pages

|Uri|Meaning|
|--|--|
|`/`|List of available build tags|
|`/t/{tag}`|List of builds with selected `{tag}`|
|`/t/{tag}/r/{repository}`|List of artifacts for build from `{repository}` with `{tag}`|
|`/b/{buildId}`|List of artifacts for build `{buildId}`|
|`/t/{tag}/r/{repository}/a/{artifact}`|List of files in `{artifact}` for build from `{repository}` with `{tag}`|
|`/b/{buildId}/a/{artifact}`|List of files in `{artifact}` for build `{buildId}`|
|`/t/{tag}/r/{repository}/a/{artifact}/f/{filePath}`|Displays the file content of `{filePath}` from `{artifact}` for build from `{repository}` with `{tag}`|
|`/b/{buildId}/a/{artifact}/f/{filePath}`|Displays the file content of `{filePath}` from `{artifact}` for build `{buildId}`|
