# Storyder
Un lecteur et éditeur de livre dont vous êtes le héros

## Usage

### Excel
L'histoire peut être entièrement rédigée dans un fchier Excel, qui sera chargé par l'application.

#### Colonnes
 - *ID* : L'identifiant unique de ce paragraphe dans l'histoire.
 - *Text* : Le paragraphe de ce moment de l'histoire affiché au joueur.
 - *DevNotes* : Des notes aue peuvent utiiser les développeurs pour ne pas se perdre.
 - *Choices* : Les choix que peut faire le joueur a cet embranchement de l'histoire.
 Si aucun, alors c'est la fin de l'histoire.
 Si un unique choix sans texte, alors le texte "Continue" est utilisé.
 format : <LAB>:Text(;<LAB>:Text)*
 - *Effects* : Les effets spéciaux qui sont effectués avant l'affichage du texte.

#### Effects
The differents effets sont appliqués avant l7affichage du texte, et dans l'ordre d'implementation.

##### Nomenclature
 filepath : un chemin de fichier relatif au dossier contenant le fichier de sauvegarde.
 varpath : un chemin de variable dans l'arbre de fiche de personnage.
 <val> : une valeur entiere ou du texte.
 <color> : code couleur HTML (HEX RRGGBB: #ff5733) ou nom de couleur tel que demande par [Godot](https://docs.godotengine.org/en/stable/classes/class_color.html#class-color-method-from-string).

##### Commandes

 - ADD:<varpath>:<val> : Adds the given value to the variable at <varpath>. Sum if Integer, concatenation if string.
 - SET:<varpath>:<val> : set the given value to the variable at <varpath>. Creates if doesn't exist.
 - REM:<varpath>:<val> : remove the given variable at <varpath>.
 - ADD_i:varpath:<val> : The given value is appended at the end of the given iterable variable.
 - SET_i:varpath:<val> : Initializes the iterable at <varpath> with the given value.
 - REM_i:varpath:<val> : Removes the given value from the iterable at <varpath>.

 - PICT:<filepath>(:TRANSITION(:delay)) : Displays the picture at <filepath> with the given transition delay in ms. If filepath is empty, remove the current picture.
  - SET : just displays the image. (default)
  - FADE : fading transition. default delay is 1000ms.
 - PICT:<color>(:TRANSITION(:delay)) : Instead of a picture, creates a uniform color screen.
 - PICT_LEFT:<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the left third of the screen.
 - PICT_CENTER:<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the center third of the screen.
 - PICT_RIGHT:<filepath>(:TRANSITION(:delay)) : same as PICT, but centers on the right third of the screen.

 - SKIP(:delay(:AFTER)): Continues to the next story paragraph as soon as every effect has been actuated. "AFTER" is for stating the delay count after the text display.
 - APPEND:<text> : Appends the given text at the end of displayed paragraph.
 - CHOICE:<choice> : Appends the given choice to the default onces.
 - GOTO:<ID> : continues to the paragraph with the given ID.
 - APPLY:<ID> : applies the effects of paragraph with the given ID.
 - TEXTONLY(:<on/off>) : removes the picture space from the GUI for text only design. Setting a picture automatically deactivates this parameter. 'on' by default if no argument given.

 - MUSIC:<filepath>(:TRANSITION(:delay)) : Plays the music at <filepath> with the given transition delay in ms. If filepath is empty, stops the current music.
  - SET : just plays the music and stops avery previous ones. (default)
  - FADE : fading transition. default delay is 1000ms.
  - ADD : Plays the music on top of already played ones.
TODO : MUSIC LOOP TYPE(Simple, repeatX, repeatDelay, pitch...)

 - IF:<condition>:<effect> : only one effect (use GOTO and APPLY for more complex stuff).
  - <condition> : <varpath>=<val> : the variable is equal to the given value.
  - <condition> : <varpath>!<val> : the variable is different from the given value.
  - <condition> : <varpath><<val> : the variable is inferior to the given value.
  - <condition> : <varpath>><val> : the variable is superior to the given value.

TODO : random and skill test related commands
TODO : combat related commands
TODO : GAME_OVER/VICTORY
TODO : post Commands