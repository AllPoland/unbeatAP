## In UNBEATABLE v2.0.x, they changed how challenge unlocks worked, causing the AP rating triggering challenge board unlocks and the 10* achievement. If you've been affected, follow the guide below

### Removing Challenges
if you've been affected by star rating challenges unlocking because of the archipelago, here's how to manually remove the unlocks  

navigate to `C:/users/[USERNAME]/AppData/LocalLow/D-CELL GAMES/UNBEATABLE/PROFILES/[LONGSTRINGOFCHARACTERS]/CHABO`, and open `37ee79d67dc9f8df38db5ae7c0e3f7e2.json` in a text editor.

**you should probably make a copy of this file before editing it, just in case you mess something up!!**

this file is very long and very messy, but all you have to do is search the ids of the challenges you are trying to remove.

list of id's of all star rating unlocks:
```
12.5*: af57a7d6bef0bd872dd2de5e341b1c36
12*: 552ebd229b49cfcf5ab371458c3e2750
11*: 33e6b1b03477db4a413cac5182c0f6ef
10*: a534b2021e1839a48683dc0d2b5a1279
9*: c211550d44aeabd6824dffcb1a41f394
8*: 69d333ac0af6b75cb9077a373ed6d9e8
7*: 9e172cc04d471c4a72dd5bcd0b831997
6*: bccabb904be2af30eae2b1d97cbd1f7f
5*: dbb273afb164060df356b67b2f8fd660
4*: 53370b07e67807a60e3952170f10839e
3*: bfa1b9d67704f3159da443ba27f7b135
2*: 63a81bf19c1d08e2d8d47a8f2eb4b306
1*: 4f13453e6bd36fc332ec14614f07cf15
```

remove all the challenges that are higher than your current star rating. to do so, search the json file for the id, set the challengeState directly after the id to 0, and statCompletions to false

example with the 12.5* challenge:  
in the file, it will look like:  
`"af57a7d6bef0bd872dd2de5e341b1c36":"{\"challengeState\":3,\"statCompletions\":[true]}"}`  
to remove the challenge it should look like  
`"af57a7d6bef0bd872dd2de5e341b1c36":"{\"challengeState\":0,\"statCompletions\":[false]}"}`

after removing all the ones you need to, save the file and open the game. once you open it, steam might tell you that the cloud save isn't synced, and if you see this click the option for "local" save and click continue. the challenges should be removed!

### Removing the 10* achievement
If you got the 10* achievement by mistake, first you **MUST** remove the challenges from your save, as shown above.  

After the challenges are removed, make sure you have [v0.6.0-alpha](https://github.com/AllPoland/unbeatAP/releases/tag/v0.6.0-alpha) of the mod installed. In the configuration file located at `[GAMEDIR]/BepInEx/config/unbeatAP.cfg`, set `Remove 10* Achievement` to `true` and boot up the game. Note that this config option will not exist in any version except for v0.6.0-alpha.

Once you boot up the game and get to the title screen, the achievement should be removed from your Steam account. The config option will automatically set itself back to false, and if you downgraded to `v0.6.0-alpha` from a newer version of the mod, you can now update safely. 

If you need further help, reach out in the UNBEATABLE (PC) thread in the Archipelago discord, and we'll be sure to help you out.