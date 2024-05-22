from PyQt5.QtGui import QIcon
from PyQt5.QtWidgets import QApplication, QWidget, QVBoxLayout, QPushButton, QLabel, QLineEdit, QListWidget, QMessageBox
from PyQt5.QtCore import Qt, QThread, pyqtSignal
from versions import Versions
from service import download, install
import asyncio


class DownloadThread(QThread):
    download_finished = pyqtSignal(str)
    download_progress = pyqtSignal(int, int)

    def __init__(self, url, progress_callback=None):
        super().__init__()
        self.url = url
        self.progress_callback = progress_callback

    def run(self):
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        file_path = loop.run_until_complete(download(self.url, self.progress_callback))
        self.download_finished.emit(file_path)


class InstallThread(QThread):
    install_finished = pyqtSignal()
    install_progress = pyqtSignal(int, int)

    def __init__(self, path, downloaded_file_path, progress_callback=None):
        super().__init__()
        self.path = path
        self.downloaded_file_path = downloaded_file_path
        self.progress_callback = progress_callback

    def run(self):
        install(self.path, self.downloaded_file_path, self.progress_callback)
        self.install_finished.emit()


class GTNHManager(QWidget):
    def __init__(self):
        super().__init__()

        self.install_thread = None
        self.download_thread = None
        self.setWindowTitle("GTNH Manager 0.4")
        self.setWindowIcon(QIcon('icon.ico'))

        self.layout = QVBoxLayout()

        self.label = QLabel("MultiMC launcher path:")
        self.layout.addWidget(self.label)

        self.input_field = QLineEdit()
        self.layout.addWidget(self.input_field)

        self.version_list = QListWidget()
        self.versions = Versions()
        version_keys = sorted(self.versions.get_dict().keys(), reverse=True)
        self.version_list.addItems(version_keys)
        self.layout.addWidget(self.version_list)

        self.install_button = QPushButton("Install")
        self.install_button.clicked.connect(self.on_install_clicked)
        self.layout.addWidget(self.install_button)

        self.status_label = QLabel("Idle")
        self.status_label.setAlignment(Qt.AlignCenter)
        self.layout.addWidget(self.status_label)

        self.setLayout(self.layout)

    def on_install_clicked(self):
        selected_version = self.version_list.currentItem()
        if selected_version is None:
            QMessageBox.warning(self, "Error", "Please select a version")
            return

        download_url = self.versions.get_from_dict(selected_version.text())
        destination_path = self.input_field.text().strip()
        if not destination_path:
            QMessageBox.warning(self, "Error", "Please enter a path")
            return

        self.status_label.setText("Downloading...")
        self.download_thread = DownloadThread(download_url, self.on_download_progress)
        self.download_thread.download_finished.connect(self.on_download_finished)
        self.download_thread.download_progress.connect(self.on_download_progress)
        self.download_thread.start()

    def on_download_progress(self, bytes_read, total_size):
        percentage = bytes_read / total_size * 100
        self.status_label.setText(f"Downloading... {percentage:.2f}%")

    def on_download_finished(self, file_path):
        self.status_label.setText("Installing...")
        destination_path = self.input_field.text().strip()
        self.install_thread = InstallThread(destination_path, file_path, self.on_install_progress)
        self.install_thread.install_finished.connect(self.on_install_finished)
        self.install_thread.install_progress.connect(self.on_install_progress)
        self.install_thread.start()

    def on_install_progress(self, files_extracted, total_files):
        percentage = files_extracted / total_files * 100
        self.status_label.setText(f"Installing... {percentage:.2f}%")

    def on_install_finished(self):
        self.status_label.setText("Done")


if __name__ == "__main__":
    app = QApplication([])

    window = GTNHManager()
    window.show()

    app.exec_()