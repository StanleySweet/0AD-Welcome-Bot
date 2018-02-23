[![Build Status](https://travis-ci.org/StanleySweet/0-A.D.-Welcome-Bot.svg?branch=master)](https://travis-ci.org/StanleySweet/0-A.D.-Welcome-Bot)
# 0-A.D.-Welcome-Bot
A simple bot that attemps to get people to stick around long enough for us to give them feedback on IRC

# Requirements
On Linux:
- gettext
- libcurl4-openssl-dev
- libicu-dev
- libssl-dev
- libunwind8
- zlib1g
- export DOTNET_INSTALL_DIR="$PWD/.dotnetcli"
- curl -sSL https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.sh | bash /dev/stdin --version "$CLI_VERSION" --install-dir "$DOTNET_INSTALL_DIR"
- export PATH="$DOTNET_INSTALL_DIR:$PATH"  
On Windows:
- DotnetCore Tools
# Installation

Compile the project or use the releases.
Run
```sh
dotnet Stan_sWfgIRCBot.dll
```
