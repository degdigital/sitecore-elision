PUSHD "%~dp0..\"
echo ^<?xml version="1.0" encoding="utf-8" ?^>>build\HedgehogDevelopment\SitecoreProject\v9.0\TDSLicense.config
echo ^<license Owner="%1" Key="%2" /^>>>build\HedgehogDevelopment\SitecoreProject\v9.0\TDSLicense.config
POPD
