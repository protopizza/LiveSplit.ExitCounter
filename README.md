## LiveSplit.ExitCounter
Exit counter for SMW romhacks. Based on the LiveSplit.Counter component.

## Setup
Download the`LiveSplit.ExitCounter.dll` file from the release, then place it within the `Components` directory of your LiveSplit folder.

## Usage
You can now add `Exit Counter` as a component in your layout.

With `Use Auto Total Count` checked, it will set the total number of exits to the number of segments in your split. Turn this off to have a manual exit count.

By default, it will use the segment index as the current exit count, **unless** the name of the split is a number, in which case it will just use the number.

Can be used in conjunction with my [LiveSplit fork](https://github.com/protopizza/LiveSplit).

## Packages / Requirements

- If you plan on building this solution yourself, you may need to add references to the following dlls (the included versions may be outdated), all of which are distributed with LiveSplit: [http://livesplit.org/](http://livesplit.org/ "Livesplit Home").
	+ LiveSplit.Core.dll
	+ UpdateManger.dll
	+ WinformsColor.dll
