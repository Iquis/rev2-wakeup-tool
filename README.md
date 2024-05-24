# rev2-wakeup-tool

[![License](https://img.shields.io/github/license/iquis/rev2-wakeup-tool?style=flat-square)](https://github.com/iquis/rev2-wakeup-tool/blob/master/LICENSE)
[![Downloads](https://img.shields.io/github/downloads/iquis/rev2-wakeup-tool/total?style=flat-square)](https://github.com/iquis/rev2-wakeup-tool/releases/latest)
[![Donate](https://shields.io/badge/ko--fi-support%20me-green?logo=ko-fi&style=flat-square)](https://ko-fi.com/iquis)
[![Donate](https://shields.io/badge/paypal-support%20me-green?logo=paypal&style=flat-square)](https://paypal.me/Iquisiquis)
[![Donate](https://shields.io/badge/patreon-support%20me-green?logo=patreon&style=flat-square)](https://patreon.com/Iquis)

![rev2-wakeup-tool Logo](.github/logo/Haehyun.png "Wakeup tool Logo")

Revesal-timing inputs for Guilty Gear Xrd: Revelator 2 in training mode.

# How to Use:

1. Open Rev 2, then start training mode.
2. Run the program.
3. The input format is as follows: numpad notation, seperate all inputs with commas, one frame per input, directions before buttons.

On the input you want to be on the first frame possible after waking up, start the input with '!', followed by the rest of the input as normal.

For example, to program Axl to do a wakeup DP (623S), you would want the following input sequence:

6,2,!3S

Or, if you want wakeup 6P throw OS (6P+H), it would look like this:

!6PH

No + signs.  Heavy slash is abbreviated "H".  

4. Check the slot you want to overwrite is the one currently selected in the program.

5. Hit "Enable" and you should be in buisness.  

# Known Issues

Nothing currently.

## Contributors
<a href="https://github.com/Iquis/rev2-wakeup-tool/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Iquis/rev2-wakeup-tool" />
</a>

Made with [contrib.rocks](https://contrib.rocks).
