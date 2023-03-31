Zunda Teller
==============

ずんだテラーは、東北応援キャラクター[東北ずん子](https://zunko.jp/)の関連キャラクターである**ずんだもん**が、その場で生成された様々な「おはなし」を読み上げてくれるアプリケーションです。

おはなしの生成に[OpenAI API](https://openai.com/blog/openai-api)、ボイスの生成に[VOICEVOX](https://voicevox.hiroshiba.jp/)を利用しています。

機能
-------
主に2つの遊び方があります。

### おはなし選択モード
ずんだもんがおはなしの候補を複数考えて話してくれるモードです。
表示されたタイトルから聞きたいおはなしを選びます。

### 自分でタイトルを考えてみるモード
自分で考えたタイトルの内容をずんだもんが考えて話してくれるモードです。
無理難題なタイトルでもずんだもんが頑張ってくれます。

インストールとセットアップ
------

### インストール 
このリポジトリをCloneしてUnityで開くか、[Release](https://github.com/mikito/zunda-teller/releases)ページからビルド済みのアプリをダウンロードしてください。

現バージョンではユーザー自身に各種サービスとの連携をお願いしており、システムとして完結していないためストア等での配信は行ってません。

### セットアップ
アプリ起動後の設定画面で、OpenAI APIとVOICEVOXとの連携のための設定を行なってください。
詳しくは以下のwikiを参考にしてください。

- [wiki/設定方法](https://github.com/mikito/zunda-teller/wiki#%E8%A8%AD%E5%AE%9A%E6%96%B9%E6%B3%95)

利用に際しての注意事項
-----
本アプリケーションで生成したコンテンツについては、以下の規約に従ってください。

- [OpenAI利用規約](https://openai.com/policies/terms-of-use)
- [VOICEVOX利用規約](https://voicevox.hiroshiba.jp/term/)
- [ずんだもん、四国めたん、九州そら音源利用規約](https://zunko.jp/con_ongen_kiyaku.html)
- [東北ずん子プロジェクト　キャラクター利用の手引き](https://zunko.jp/guideline.html)

ライセンス
-------
本リポジトリに含まれるプログラムやアセットはMITライセンスです。

ただし、以下の外部フォントはApache License Version 2.0でライセンスされています。
- ぼくたちのゴシック2(http://fontopo.com)

Apache License Version 2.0の詳細については、[LICENSE-APACHE-2.0ファイル](https://github.com/mikito/zunda-teller/blob/master/LICENSE-APACHE-2.0)を参照してください。

また、ずんだもん立ち絵イラストは以下素材の一部構造を修正して利用しています。
- [ずんだもん立ち絵素材](https://seiga.nicovideo.jp/seiga/im10788496?ref=nicoms)(@sakamoto_AHR)