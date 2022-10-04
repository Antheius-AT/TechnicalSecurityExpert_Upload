#!/usr/bin/env python3
__author__ = "Gregor Faiman"
__copyright__ = "Copyright 2020, FHWN"
__credits__ = ["Gregor Faiman"]
__email__ = "Gregor.Faiman@fhwn.ac.at"

import os
import datetime


class ErrorCallback:
    """
    Class serving as a container for determining whether command line arguments are valid,
    and storing custom error messages in case of invalid arguments.
    """

    def __init__(self, errorMessage, isValid):
        """
        Initializes a new instance of the ErrorCallback class.

        errorMessage: The custom error message.
        isValid: Whether the arguments are valid.
        """

        self.errorMessage = errorMessage
        self.isValid = isValid


def ValidateArguments(commandLineArguments):
    """
    Validates the command line arguments.
    Returns an error callback storing information about the state of the result of the error checks.

    commandLineArguments: The command line arguments.
    """

    path = commandLineArguments[0]
    callBack = ErrorCallback("Arguments invalid!", False)
    isArgumentCountValid = len(commandLineArguments) == 1
    isArgumentPathToDirectory = os.path.isdir(path)
    isArgumentPathToFile = os.path.isfile(path)

    if not isArgumentCountValid:
        callBack.errorMessage = 'Please specify the path to an existing directory as a single argument.'
    elif not (isArgumentPathToFile or isArgumentPathToDirectory):
        callBack.errorMessage = 'Please specify an existing path to a directory.'
    elif not isArgumentPathToDirectory:
        callBack.errorMessage = 'Please specify a path to a directory, not a file!'
    else:
        callBack.errorMessage = 'No errors detected.'
        callBack.isValid = True

    return callBack


def CreateDirectory(directoryPath):
    """
    This function checks whether a backup directory for todays date is already in existence
    and if it isn`t proceeds to create it.

    directoryPath: The root directory hosting the daily archive directories.
    """

    archiveRootPath = os.path.join(directoryPath, datetime.date.today().__str__())
    archiveHelpImagesFolderPath = os.path.join(archiveRootPath, 'Hilfe')
    archiveStandardImagesFolderPath = os.path.join(archiveRootPath, 'Tagesbilder')

    if os.path.exists(archiveRootPath):
        print("Could not create archive, as directory" + archiveRootPath + "already exists!")
        return

    os.mkdir(archiveRootPath)
    os.mkdir(archiveHelpImagesFolderPath)
    os.mkdir(archiveStandardImagesFolderPath)

    print(
        'Directory' + archiveRootPath + 'containing subdirectories:\n' + archiveHelpImagesFolderPath + '\n' + archiveStandardImagesFolderPath + '\nwas successfully created.')
