import os
import tempfile
import aiohttp
import aiofiles
import zipfile
import logging
import re

logging.basicConfig(filename='app.log', filemode='w', format='%(name)s - %(levelname)s - %(message)s')


async def download(url, progress_callback=None):
    try:
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as resp:
                file_name = os.path.basename(url.split("?")[0])
                temp_dir = tempfile.gettempdir()
                file_path = os.path.join(temp_dir, file_name)

                size = int(resp.headers.get('content-length', 0))
                bytes_read = 0

                async with aiofiles.open(file_path, 'wb') as f:
                    while True:
                        chunk = await resp.content.read(1024)
                        if not chunk:
                            break

                        await f.write(chunk)
                        bytes_read += len(chunk)

                        if progress_callback is not None:
                            progress_callback(bytes_read, size)

                return file_path
    except aiohttp.ClientResponseError as e:
        logging.error("ClientResponseError occurred", exc_info=True)
    except Exception as e:
        logging.error("Unexpected exception occurred", exc_info=True)


def install(path, downloaded_file_path, progress_callback=None):
    try:
        if not os.path.exists(path):
            raise Exception(f"Path {path} does not exist")

        instances_path = os.path.join(path, "instances")
        if not os.path.exists(instances_path):
            os.makedirs(instances_path)

        if not downloaded_file_path.endswith(".zip"):
            raise Exception(f"File {downloaded_file_path} is not a zip file")

        with zipfile.ZipFile(downloaded_file_path, 'r') as zip_ref:
            total_files = len(zip_ref.infolist())
            extracted_files = 0

            for file in zip_ref.infolist():
                zip_ref.extract(file, instances_path)
                extracted_files += 1

                if progress_callback is not None:
                    progress_callback(extracted_files, total_files)

        # Extract the version from the downloaded file path
        match = re.search(r'GT_New_Horizons_([\d\.]+)_', os.path.basename(downloaded_file_path))
        if match:
            version = match.group(1)
        else:
            raise Exception(f"Could not extract version from {downloaded_file_path}")

        # Call the configure function after installation is complete
        configure(path, version)

        os.remove(downloaded_file_path)
    except zipfile.BadZipFile as e:
        logging.error("BadZipFile occurred", exc_info=True)
    except Exception as e:
        logging.error("Unexpected exception occurred", exc_info=True)


def configure(path, version):
    try:
        # Construct the path to the instance.cfg file
        cfg_file_path = os.path.join(path, "instances", f"GT New Horizons {version}", "instance.cfg")

        # Check if the file exists
        if not os.path.isfile(cfg_file_path):
            raise Exception(f"File {cfg_file_path} does not exist")

        # Define the content to be written to the file
        content = f"""
ForgeVersion=
InstanceType=OneSix
IntendedVersion=
JavaArchitecture=64
JavaTimestamp=1713342035000
JavaVersion=21.0.3
JoinServerOnLaunch=false
JvmArgs=-Dfile.encoding=UTF-8 -Djava.system.class.loader=com.gtnewhorizons.retrofuturabootstrap.RfbSystemClassLoader -Djava.security.manager=allow --add-opens java.base/jdk.internal.loader=ALL-UNNAMED --add-opens java.base/java.net=ALL-UNNAMED --add-opens java.base/java.nio=ALL-UNNAMED --add-opens java.base/java.io=ALL-UNNAMED --add-opens java.base/java.lang=ALL-UNNAMED --add-opens java.base/java.lang.reflect=ALL-UNNAMED --add-opens java.base/java.text=ALL-UNNAMED --add-opens java.base/java.util=ALL-UNNAMED --add-opens java.base/jdk.internal.reflect=ALL-UNNAMED --add-opens java.base/sun.nio.ch=ALL-UNNAMED --add-opens jdk.naming.dns/com.sun.jndi.dns=ALL-UNNAMED,java.naming --add-opens java.desktop/sun.awt=ALL-UNNAMED --add-opens java.desktop/sun.awt.image=ALL-UNNAMED --add-opens jdk.dynalink/jdk.dynalink.beans=ALL-UNNAMED --add-opens java.sql.rowset/javax.sql.rowset.serial=ALL-UNNAMED
LWJGLVersion=
LiteloaderVersion=
LogPrePostOutput=true
MCLaunchMethod=LauncherPart
ManagedPack=false
ManagedPackID=
ManagedPackName=
ManagedPackType=
ManagedPackVersionID=
ManagedPackVersionName=
OverrideCommands=false
OverrideConsole=false
OverrideGameTime=false
OverrideJavaArgs=true
OverrideJavaLocation=false
OverrideMCLaunchMethod=false
OverrideMemory=false
OverrideNativeWorkarounds=false
OverrideWindow=false
iconKey=default
lastLaunchTime=1716477359280
lastTimePlayed=9008
name=GTNH {version}
notes=
totalTimePlayed=26810
        """.strip()

        # Write the content to the file
        with open(cfg_file_path, 'w') as file:
            file.write(content)

        logging.info("Configuration completed successfully")
    except Exception as e:
        logging.error("Unexpected exception occurred during configuration", exc_info=True)
