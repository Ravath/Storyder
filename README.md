# Storyder
Un lecteur et éditeur de livre dont vous êtes le héros

## Usage

### Dependences
 - Powered by Godot
 - Weaver library

### Excel
L'histoire peut être entièrement rédigée dans un fichier Excel, qui sera chargé par l'application.

#### Colonnes
 - *ID* : L'identifiant unique de ce paragraphe dans l'histoire.
 - *Text* : Le paragraphe de ce moment de l'histoire affiché au joueur.
 - *DevNotes* : Des notes aue peuvent utiiser les développeurs pour ne pas se perdre.
 - *Choices* : Les choix que peut faire le joueur a cet embranchement de l'histoire.
 Si aucun, alors c'est la fin de l'histoire.
 Si un unique choix sans texte, alors le texte "Continue" est utilisé.
 format : \<LAB>:Text(;\<LAB>:Text)*
 - *Effects* : Les effets spéciaux qui sont effectués avant l'affichage du texte.

#### Effects
The differents effets sont appliqués avant l'affichage du texte, et dans l'ordre d'implementation.
Les PostEffects sont des effets appliqués après que le joueur est fait son choix et avant de passer au paragraphe suivant.

##### Nomenclature
 filepath : un chemin de fichier relatif au dossier contenant le fichier de sauvegarde.
 varpath : un chemin de variable dans l'arbre de fiche de personnage.
 \<val> : une valeur entiere ou du texte.
 \<color> : code couleur HTML (HEX RRGGBB: #ff5733) ou nom de couleur tel que demandé par [Godot](https://docs.godotengine.org/en/stable/classes/class_color.html#class-color-method-from-string).

##### General Commands

x- SKIP(:delay): Continues to the next story paragraph as soon as every effect has been actuated. If pre-effect, requires to have only one continue choice.
 - APPEND:\<text> : Appends the given text at the end of displayed paragraph.
 - CHOICE:\<LAB>:Text : Appends the given choice to the default onces.
 - GOTO:\<LAB> : continues to the paragraph with the given Label.
x- APPLY:\<LAB> : applies the effects of the paragraph with the given Label.
 - TEXTONLY(:\<on/off>) : removes the picture space from the GUI for text only design. Setting a picture automatically deactivates this parameter. 'on' by default if no argument given.

#### Picture Commands

 - PICT:\<filepath>(:TRANSITION(:delay)) : Displays the picture at \<filepath> with the given transition delay in ms. If filepath is empty, remove the current picture.
x - SET : just displays the image. (default)
x - FADE : fading transition. default delay is 1000ms.
x- PICT:\<color>(:TRANSITION(:delay)) : Instead of a picture, creates a uniform color screen.
x- PICT_LEFT:\<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the left third of the screen.
x- PICT_CENTER:\<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the center third of the screen.
x- PICT_RIGHT:\<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the right third of the screen.

#### Music Commands

 - MUSIC:\<filepath>(:TRANSITION(:delay)) : Plays the music at \<filepath> with the given transition delay in ms. If filepath is empty, stops the current music.
x - SET : just plays the music and stops avery previous ones. (default)
x - FADE : fading transition. default delay is 1000ms.
x - ADD : Plays the music on top of already played ones.
TODO : MUSIC LOOP TYPE(Simple, repeatX, repeatDelay, pitch...)

#### Variables Commands
Values can be random. use roll macros with the  '[' ']'.
See Weaver documentation on Luck module for more details.

 - SUM:\<varpath>:\<val> : Sum the given value to the variable at \<varpath>. Sum if Integer, concatenation if string.
 - SET:\<varpath>:\<val> : Set the given value to the variable at \<varpath>. Creates if doesn't exist.
 - REM:\<varpath>		 : Remove the given variable at \<varpath>.
 - ADD_i:\<varpath>:\<val> : The given value is appended at the end of the given List array.
 - SET_i:\<varpath>:\<val> : Initializes the List array at \<varpath> with the given value.
 - REM_i:\<varpath>:\<val> : Removes the given value from the List array at \<varpath>.

#### Condition Commands
Values can be random. use roll macros with the  '[' ']'.
See Weaver documentation on Luck module for more details.
Values can be \<varpath> : the value of the variable at the given path will be tested.

 - IF:\<condition>;\<effect>;\<else_effect> : only one effect (use GOTO and APPLY for more complex stuff). See Weaver.Heroes.Destiny's Readme for more details on how the parser works.
   - \<condition> : \<val>==\<val> : the variable is equal to the given value.
   - \<condition> : \<val>!=\<val> : the variable is different from the given value.
   - \<condition> : \<val>\<\<val> : the variable is inferior to the given value.
   - \<condition> : \<val>\>\<val> : the variable is superior to the given value.
   - \<condition> : \<val>\<=\<val> : the variable is inferior or equal to the given value.
   - \<condition> : \<val>\>=\<val> : the variable is superior or equal to the given value.
 - CONTAINS:\<varpath>:\<val>;\<effect>;\<else_effect> : A condition testing if the given array variable contains the given value.

##### Structure of normal IF
A __IF__ command will execute the first following command if true, the second if false. 
They will not be executed by any other mean.

>IF:*condition*;<br>
&emsp;IfTrueEffect;<br>
&emsp;IfFalseEffect;

If the parser fails to find two commands following a condition, an error will arise. To prevent that, use empty commands, by letting a blank space between two __';'__.

Exemple with no *IfTrueEffect*.
>IF:*condition*;<br>
&emsp;;<br>
&emsp;IfFalseEffect;

Exemple with no *IfFalseEffect*.
>IF:*condition*;<br>
&emsp;IfTrueEffect;<br>
&emsp;;
	
##### Structure of imbricated IF

The IF structure can be stacked this way in order to use multiple conditions.

>IF1:*condition*;<br>
&emsp;IF2:*condition*; // If1TrueEffect<br>
&emsp;&emsp;If2TrueEffect;<br>
&emsp;&emsp;If2FalseEffect;<br>
&emsp;If1FalseEffect;<br>

#### Fighting Fantasy System Commands

The FFS implements some specific commands for convenience of use.

 - LUCK : A standard test against luck. Decreases Luck by 1.
 - COMBAT:\<Ennemy,Hab,Str>+:\<Escape,Desc>:\<CombatType>;\<ENV>;\<VICTORY>;\<FLIGHT>; : Sets up a combat.
	This will add a "start combat" choice to the paragraph.
	Hence, the post-effects of the current paragraph will be actuated at the beginning of the combat.
   - \<Ennemy,Hab,Str>+ : One or more ennemy to fight, two ennemies definitions are separated by ','.
     - Ennemy : Ennemi's name.
	 - Hab 	  : Ennemi's hability
	 - Str 	  : Ennemi's strenght
   - \<Escape,Desc> : The number of assault to fight before allowed to escape. -1 if never. Can add a description to the escape prompt.
   - \<CombatType> : If more than one ennemy, indicates the type of combat procedure.
     - SUCCESSIVE : The first ennemi is fought 1 to 1 before fighting the second in line.
	 - TOGETHER   : Each round, the player chooses an ennemy to fight as usual. The other ennemies participate in the assault as usual, but can not receive damage.
   - following commands :  The COMBAT command acts like a Condition command, but the following 3 next commands are \<ENV>;\<VICTORY>;\<ESCAPE>;.
     - \<ENV>     : This command is performed at each assault of the combat.
     - \<VICTORY> : This command is performed when the player wins the combat.
     - \<ESCAPE>  : This command is performed when the player escapes the combat.
x- SET_COMBAT_PROXY:\<proxyname,Hab,Str> : the given creature will fight instead of the player for the upcoming fights. If defeated, the player follows up the combat as usual.
x- UNSET_COMBAT_PROXY : Removes the combat proxy.

## TODO

 - eat food/drink potion => use system check
 - GAME_OVER/VICTORY
 - use keyboard to play
 - CORRECTION : imprement a function to the conditions interface to prevent border effects when trying to read results without actually wanting to roll the dice.
 - CORRECTION : The ValueModule<list> do not update the value when changed because its a reference...
	use a ValueCollectionModule ?
 
## Credits
 - Code : __Ravath__
 - La sorciere des Neiges : __Folio Junior : Defis Fantastiques__
 - Test sounds from __Freesound__ 
 - Campfire at night soundscape (louder animals, quieter campfire) : __Silverillusionist__
 - triple-tapey remix of excerpt of LogicMoon's freesound 751734 : __Timbre__
 