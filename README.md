# Unity

##バージョン：3.6f1

# Gitルール

## リポジトリモデル

- Git Flow

![Git Flow](https://datasift.github.io/gitflow/IntroducingGitFlow.html)

## ブランチ名

例:

- release
- master
- develop
- feature/scripts/filename.py
- fix/plugins/プラグイン名

第3キーワードの例: common|script|assets|middleware|plugins|static|store|utils|test|config|lib|docs|special

## コミットルール

例:

- fix: scrapigのコンパイルエラー修正

メッセージ本文は日本語

プレフィックスは以下のルール

```txt
Type
Must be one of the following:

feat: A new feature
fix: A bug fix
docs: Documentation only changes
style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
refactor: A code change that neither fixes a bug nor adds a feature
perf: A code change that improves performance
test: Adding missing or correcting existing tests
chore: Changes to the build process or auxiliary tools and libraries such as documentation generation
```

引用元: https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#type

## プルリクエストルール

- コードを書いていない開発者のアクセプトでマージ
- セルフマージはしない

# Commentary

## フロー
- プロダクトマネージャーはプロジェクト開発と同時にリポジトリを作るor既にプロジェクトがある場合はリポジトリを作りそれにinitする
- プロダクトマネージャーは上記のmasterブランチの根元からdevelopブランチを切る
- developerはdevelopブランチをクローンする
1. developerは新しい機能を作りたいときは各々developブランチからfeatureブランチを切る
2. 切ったfeatureブランチの中で作業をする。当然この変更はfeatureのみに反映される。
3. 機能が完成したらdevelopブランチにmeargeリクエストをする
4. 全ての機能が完成したらreleaseブランチを切る。簡易的なバクを取り除くのは許容される。
5. 新しい機能など大きな変更が必要な場合ならdevelopブランチへ戻る。
- 問題なく質が保証されるならmasterブランチにmeargeリクエストをする。
- masterブランチに質が保証された製品が完成する。
