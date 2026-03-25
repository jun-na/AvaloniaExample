#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
PROJECT_FILE="$ROOT_DIR/AvaloniaExample.csproj"
CONFIGURATION="${CONFIGURATION:-Release}"
TARGET_RUNTIME="${TARGET_RUNTIME:-}"
APP_NAME="${APP_NAME:-AvaloniaExample}"
VERSION="${VERSION:-1.0.0}"
OUTPUT_ROOT="${OUTPUT_ROOT:-$ROOT_DIR/artifacts/macos}"
PUBLISH_DIR="$OUTPUT_ROOT/publish"
APP_BUNDLE_DIR="$OUTPUT_ROOT/$APP_NAME.app"
CONTENTS_DIR="$APP_BUNDLE_DIR/Contents"
MACOS_DIR="$CONTENTS_DIR/MacOS"
RESOURCES_DIR="$CONTENTS_DIR/Resources"
PLIST_FILE="$CONTENTS_DIR/Info.plist"
PUBLISHED_BINARY="$PUBLISH_DIR/$APP_NAME"
APP_BINARY="$MACOS_DIR/$APP_NAME"

detect_runtime() {
    local machine
    machine="$(uname -m)"

    case "$machine" in
        arm64)
            printf '%s\n' "osx-arm64"
            ;;
        x86_64)
            printf '%s\n' "osx-x64"
            ;;
        *)
            printf 'Unsupported macOS architecture: %s\n' "$machine" >&2
            exit 1
            ;;
    esac
}

if [[ -z "$TARGET_RUNTIME" ]]; then
    TARGET_RUNTIME="$(detect_runtime)"
fi

mkdir -p "$OUTPUT_ROOT"
rm -rf "$PUBLISH_DIR" "$APP_BUNDLE_DIR"

dotnet publish "$PROJECT_FILE" \
    -c "$CONFIGURATION" \
    -r "$TARGET_RUNTIME" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=false \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -o "$PUBLISH_DIR"

if [[ ! -f "$PUBLISHED_BINARY" ]]; then
    printf 'Publish succeeded, but expected binary was not found: %s\n' "$PUBLISHED_BINARY" >&2
    exit 1
fi

mkdir -p "$MACOS_DIR" "$RESOURCES_DIR"
cp "$PUBLISHED_BINARY" "$APP_BINARY"
chmod +x "$APP_BINARY"

cat > "$PLIST_FILE" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>en</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleExecutable</key>
    <string>$APP_NAME</string>
    <key>CFBundleIdentifier</key>
    <string>com.example.$APP_NAME</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

printf 'Created app bundle: %s\n' "$APP_BUNDLE_DIR"
printf 'Published runtime: %s\n' "$TARGET_RUNTIME"
printf 'Launch with: open %q\n' "$APP_BUNDLE_DIR"