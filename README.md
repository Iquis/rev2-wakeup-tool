# rev2-wakeup-tool
Revesal-timing inputs for Guilty Gear Xrd: Revelator 2 in training mode.

# How to Use:

1. Open Rev 2, then start training mode, then pause the game.  The pause the game part is mandatory.
2. Run the program, make sure it starts up and displays the correct dummy.
3. The input format is as follows: numpad notation, seperate all inputs with commas, one frame per input, directions before buttons.

On the input you want to be on the first frame possible after waking up, start the input with '!', followed by the rest of the input as normal.

For example, to program Axl to do a wakeup DP (623S), you would want the following input sequence:

6,2,!3S

Or, if you want wakeup 6P throw OS (6P+H), it would look like this:

!6PH

No + signs.  Heavy slash is abbreviated "H".  

4. Check the slot you want to overwrite is the one currently selected in the program and in-game.

5. Hit "Enable" and you should be in buisness.  

# Known Issues (IMPORTANT, MUST READ)

1. If you are switching characters through the menu/causing training mode to reload for any reason, close the program and wait for it to close before doing so.  Otherwise, the program will be in a state where you have to close and restart gg and the tool from step 1.

2. If it's taking too long to disable, try knocking the opponent down and see if that fixes the problem.  This issue has been fixed since version 0.1.1.  

3. If for some reason you have to ctrl+alt+delete and kill the tool, don't just kill the main process, there's also another helper process called frida-winjector-32 that one of the dlls spawn that helps with the magic.  Kill that too before restarting.


## Contributors
<a href="https://github.com/Iquis/rev2-wakeup-tool/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Iquis/rev2-wakeup-tool" />
</a>

Made with [contrib.rocks](https://contrib.rocks).
