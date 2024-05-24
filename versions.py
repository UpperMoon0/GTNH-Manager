class Versions:
    _instance = None
    _dict = {}

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(Versions, cls).__new__(cls)
            cls._instance.add_to_dict("2.6.0",
                                      "https://www.dropbox.com/scl/fi/rdn46wbc3a88xvpznh86o/GT_New_Horizons_2.6.0_Java_17-21.zip?rlkey=zu4xcrlz4xlzv5krgkh707hg7&dl=1")
            cls._instance.add_to_dict("2.6.1",
                                      "https://www.dropbox.com/scl/fi/g5dw3yp8g8jg17pb3vnhz/GT_New_Horizons_2.6.1_Java_17-21.zip?rlkey=zgaey4suyoytofeuoqnj85uw7&dl=1")
        return cls._instance

    def add_to_dict(self, key, value):
        self._dict[key] = value

    def get_from_dict(self, key):
        return self._dict.get(key)

    def get_dict(self):
        return self._dict
