# NR2003DashM
Nascar Racing 2003 Dashboard based on project: https://github.com/wslates/NR2K3Dash

This project makes use of a custom NR2003.dll that helps gather telemetry
source here: https://github.com/deadfred-/NR2003

IMPORTANT:
This program must be run BEFORE nr2003 is running.  If nr2003 is already running when this program is launched, it will crash or at the very least, not load the data.  This is due to expected behavior from the nr2003 game.  When they wrote their telemetry handoff behavior it required the app to be requesting data before the game was launched so there is nothing we can do about this.
