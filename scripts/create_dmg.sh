#!/bin/bash

APP_NAME="Build/zunda-teller.app"
DMG_NAME="BUild/zunda-teller.dmg"
VOL_NAME="Zunda Teller"

hdiutil create -format UDZO -srcfolder "$APP_NAME" -volname "$VOL_NAME" "$DMG_NAME"
