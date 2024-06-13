# Escape Blitz
決められた制限時間で部屋から脱出することをできる限り繰り返す、スコアアタック形式のゲームとなっています。部屋の内容は、複数のドアの内、正しいドアを見抜く部屋、一定間隔で光る道を覚え、遠くのドアにたどり着く部屋などがあり、最終的にはただドアに向かうだけのゲームではありますが、考えることが多くあり、その上、スピーディーなゲームの流れとなっているので、満足性が高いゲームであると感じております。
# アプリケーションのリンク
以下のGoogleドライブのリンクに、このプロジェクトのexe形式のアプリケーションをzip形式で圧縮し、載せております。
[https://drive.google.com/file/d/1WPyu6hEQxW21PNZjFHLrZUNh3h6SReX-/view?usp=drivesdk](https://drive.google.com/drive/folders/1AATigI-EYCxyhhVKPDH9MLNKel9F10qQ?usp=drive_link)
# コード特徴
## 抽象クラスの活用
後に追加するかもしれない部屋の振る舞いの記述を簡単にするために、基礎的な処理をRoomInfoに記述し、抽象クラスとしています。また、他コンポーネントにおいて、部屋の種類ごとに参照方法を変える必要性を無くすために抽象メゾットの活用も行っています。
## 独自の属性の使用
部屋の難易度を調整するために、Room,LineなどのRoomInfoの派生クラスには部屋のプロパティを決定する関数が複数含まれていますが、関数の役割を明確にすることと、プロパティ間で依存関係があった場合に関数の処理順番を定義できるようにするため、C#のカスタム属性の機能を使い、｢プロパティを決める関数である｣という属性のParameterDecisionAttitudeを定義し、関数に付与できるようにしました。部屋のプロパティを決めるコードを派生クラスに全て記述するよりも可読性が高くなっていると思います。
## エディタの拡張
Class.csに含まれるLanguageStringクラスは2つのstring型のオブジェクトを持っていますが、それらにTextArea属性を付与してもインスペクタ上のテキストエリアが広がらなかったので、PropertyDrawerを用いてチェックボックスにチェックを入れるとテキストエリアが広がるように改良しました。また、独自にDebugWindowと呼ばれるエディタウィンドウを作成し、自作メッシュが正しく作られるかなどのテストを手軽にできるようにしました。
