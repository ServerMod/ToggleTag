# ToggleTag [![Build Status](https://jenkins.karlofduty.com/job/ToggleTag/job/master/badge/icon)](https://jenkins.karlofduty.comblue/organizations/jenkins/ToggleTag/activity) [![Release](https://img.shields.io/github/release/KarlofDuty/ToggleTag.svg)](https://github.com/KarlOfDuty/ToggleTag/releases) [![Downloads](https://img.shields.io/github/downloads/KarlOfDuty/ToggleTag/total.svg)](https://github.com/KarlOfDuty/ToggleTag/releases) [![Discord Server](https://img.shields.io/discord/430468637183442945.svg?label=discord)](https://discord.gg/C5qMvkj)

# Plugin has been archived as the smod api has been abandoned.

An SCP:SL ServerMod plugin which lets you persistently toggle your staff tag using the default `hidetag` and `showtag` commands. You have to use the RA console, not the local one.

Requires you to enable the text based admin commands or use scpdiscord.

Now also persistently toggles overwatch.

Has a single config entry, `toggletag_global` which decides whether the plugin files are placed in the global or local config directory.

## Installation

Extract the included zip and place the contents in `sm_plugins`.

## Commands

`console_hidetag <steamid>`

`console_showtag <steamid>`

These are not meant to be used directly, they just exist for SCPDiscord integration. You can instead just use the default `hidetag` and `showtag` commands.

## Permissions

`toggletag.savetag` - Allows players to have their tag status saved, given by default.

`toggletag.saveoverwatch` - Allows players to have their overwatch status saved, given by default.
