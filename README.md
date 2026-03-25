# AvaloniaExample

Avalonia UI を使った macOS 向けデスクトップアプリのサンプルプロジェクトです。  
ターゲットフレームワーク: **.NET 10**, UI ライブラリ: **Avalonia 11**

## 必要なもの

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- macOS (Apple Silicon または Intel)

## 開発時の実行

```bash
dotnet run
```

## macOS 向けアプリバンドルの作成

`scripts/package-macos-app.sh` を実行すると、`artifacts/macos/` 以下に `.app` バンドルが生成されます。

```bash
./scripts/package-macos-app.sh
```

実行後の出力ディレクトリ構成:

```
artifacts/macos/
├── AvaloniaExample.app/       # macOS アプリバンドル
│   └── Contents/
│       ├── Info.plist
│       ├── MacOS/
│       │   └── AvaloniaExample    # 実行バイナリ
│       └── Resources/
└── publish/
    └── AvaloniaExample            # dotnet publish の中間出力
```

アプリを起動するには:

```bash
open artifacts/macos/AvaloniaExample.app
```

### オプション

環境変数でビルド設定を変更できます。

| 変数 | デフォルト値 | 説明 |
|---|---|---|
| `CONFIGURATION` | `Release` | ビルド構成 |
| `TARGET_RUNTIME` | 自動検出 | ランタイム識別子 (`osx-arm64` / `osx-x64`) |
| `APP_NAME` | `AvaloniaExample` | アプリ名 |
| `VERSION` | `1.0.0` | バンドルバージョン |
| `OUTPUT_ROOT` | `artifacts/macos` | 出力ディレクトリ |

例:

```bash
VERSION=2.0.0 ./scripts/package-macos-app.sh
```

> **Note**: スクリプトはアーキテクチャを自動判定します。Apple Silicon では `osx-arm64`、Intel Mac では `osx-x64` でビルドされます。

## プロジェクト構成

```
AvaloniaExample/
├── App.axaml / App.axaml.cs       # アプリケーション定義
├── MainWindow.axaml / .cs         # メインウィンドウ
├── ViewModels/                    # ViewModel 群
├── scripts/
│   └── package-macos-app.sh      # macOS パッケージングスクリプト
└── artifacts/                     # ビルド成果物 (gitignore)
```
