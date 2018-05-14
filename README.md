# ECOLOGMobileApp

**研究室内でのコードレビュアーを募集中!!**
## 概要
卒研再現最終発表に向けて、TOD2016川沼さんが作成したものを再実装。  
[このリポジトリ](https://github.com/y-kisse/TOD2017MobileApp)に存在するが、Xamarinのバージョンの問題か、実行時エラーが解決できなかった。  

## 方針
View、ViewModelは最新のXamarinドキュメントを参照しながら書き直しを行う。  
ModelはRealmのドキュメントを見ながら作成。  

ECOLOGのロジック部分については、SensorLogInserterReのロジック部分をそのまま持ってくる。  

## コードのルール
改行コードは**LF**に統一する。  
VS2017では、**Line Endings Unifier**という拡張機能をインストールする。   

## コミットメッセージ
以下の記事が参考になる。  
自分の流行りよって表記のゆれが発生することは少なくしたい。  
- [gitにおけるコミットログ/メッセージ例文集100](https://gist.github.com/mono0926/e6ffd032c384ee4c1cef5a2aa4f778d7)
- [Gitコミットメッセージの書き方](https://postd.cc/how-to-write-a-git-commit-message/)
- [Angular.jsのコミットメッセージのガイドライン](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#commits)

今回は以下のコミットプレフィクスを試してみよう。  
Must be one of the following:

* **feat**: A new feature
* **fix**: A bug fix
* **docs**: Documentation only changes
* **style**: Changes that do not affect the meaning of the code (white-space, formatting, missing
  semi-colons, etc)
* **refactor**: A code change that neither fixes a bug nor adds a feature
* **perf**: A code change that improves performance
* **test**: Adding missing or correcting existing tests
* **chore**: Changes to the build process or auxiliary tools and libraries such as documentation
  generation