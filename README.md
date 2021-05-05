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

as well as `AzureAd` section.

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

## Globbing

### Artifacts
It is possible to select artifact name using `*` wildcard. The artifact with matching name and the highest ID will get selected, where the intention of using ID ordering is to pick up the lastest matching file.

The solution is designed to be working with multiple stage reruns that generates artifacts of the same type with different suffixes, like:
* `tests-attempt1`
* `tests-attempt2`

Specifying `tests-*` will match the one with highest ID, hopefully `tests-attempt2`.

### Zip file content
It is possible to navigate through the zip file content and use `*` to select a file with latest lastWrite date.
The `*` can be used to match path fragments, but it won't work to match path separator, i.e. `/`.

Example usage: `/t/my-tag/r/my-repo/a/tests-attempt*/f/tests-attempt*/*.html`