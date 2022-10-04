#!/usr/bin/env python3
__author__ = "Gregor Faiman"
__copyright__ = "Copyright 2020, FHWN"
__credits__ = ["Gregor Faiman", "Daniela Sander"]
__email__ = "Gregor.Faiman@fhwn.ac.at"

import os
import smtplib
from email.mime.text import MIMEText
from email.mime.image import MIMEImage
from email.mime.multipart import MIMEMultipart
from email.mime.application import MIMEApplication

from re import search


def ValidateDirectoryContents(imageSourceDirectory):
    """
    This function reads the contents of the specified directory and checks whether at least one of the contained
    files is a jpg or png image file.

    imageSourceDirectory: The specified directory that is searched.
    """

    for root, dirs, files in os.walk(imageSourceDirectory):
        for file in files:
            if not ('.jpg' in file or '.png' in file):
                continue

            return True

    return False


def ValidateReceivers(receivers):
    """
    Iterates through the list and checks whether it contains at
    least one valid E-Mail address, using a regex filter that looks
    for the pattern: xxx@xxx.xxx

    receivers: The receiver mail addresses.
    """

    # Regular expression filtering for: xxx@xxx.xxx
    regex = '[^@]+@[^@]+\.[^@]+'
    for currentAddress in receivers:
        if not search(regex, currentAddress):
            continue

        return True

    return False


def ValidateArgumentSemantics(receivers, pathToImageSourceDirectory):
    """
    This function checks whether the specified paths are valid paths.
    If they are valid paths, the function determines whether the contents of the
    source directory as well as the receivers file are valid, and returns this value.

    receivers: List containing receiver E-Mail addresses.
    pathToImageSourceDirectory: The path to the directory containing image files.
    """

    isImageSourceDirectoryPathValid = os.path.exists(pathToImageSourceDirectory) and os.path.isdir(
        pathToImageSourceDirectory)
    isReceiversListValid = ValidateReceivers(receivers)

    isImageSourceDirectoryValid = isImageSourceDirectoryPathValid and ValidateDirectoryContents(
        pathToImageSourceDirectory)

    return isImageSourceDirectoryValid and isReceiversListValid


def ValidateArgumentCount(commandLineArgs):
    """
    This method validates whether this script was launched with enough arguments
    for determining whether the arguments themselves are valid or not.

    commandLineArgs: The command line arguments.
    """

    if len(commandLineArgs) == 4:
        return True


def ValidateArguments(commandLineArgs):
    """
    This method validates arguments by making various function calls to check the amount of arguments
    as well as the contents of the arguments.

    commandLineArgs: Command line arguments.
    """

    return ValidateArgumentCount(commandLineArgs) and ValidateArgumentSemantics(commandLineArgs[2], commandLineArgs[3])


def ExtractReceivers(pathToReceiversFile):
    """
    This method extracts a list of message receivers from a specified text file and returns that list.

    pathToReceiversFile: The path to the file containing the receiving E-Mail addresses.
    """

    receiversList = []
    regex = '[^@]+@[^@]+\.[^@]+'

    with open(pathToReceiversFile, 'rt') as fileStream:
        while True:
            currentAddress = fileStream.readline()

            if currentAddress == "":
                fileStream.flush()
                fileStream.close()
                break
            elif currentAddress == '\n' or not search(regex, currentAddress):
                continue
            else:
                receiversList.append(currentAddress.strip())
                continue

    return receiversList


def BuildMessage(sourceAddress, receivers, sourceDirectory):
    """
     This function builds the E-Mail message. It receives sender, destination, as well as the directory
     containing the images as arguments, and proceeds to use this information to build the message, and
     attach images.

     sourceAddress: The sender address.
     receivers: The list of receivers.
     sourceDirectory: The directory containing images that are to be attached.
    """

    message = MIMEMultipart()
    message['From'] = sourceAddress
    message['To'] = ', '.join(receivers)
    message['Subject'] = 'Hilfe potentiell benötigt!'
    message.attach(MIMEText('Bilder der aktuellen Situation finden sich im Anhang!', 'plain'))

    for root, dirs, files in os.walk(sourceDirectory):
        for file in files:
            if not ('.jpg' in file or '.png' in file):
                continue
            else:
                fullPath = os.path.join(sourceDirectory, file)
                print(fullPath)
                fp = open(fullPath, 'rb')
                image = MIMEImage(fp.read())
                fp.close()
                message.attach(image)

    return message


def BuildMessagePDF(senderMail, receivers, pdfPath):
    """
    Builds a mail message containing a PDF as attachment.

    senderMail: The E-Mail address of the sender.
    receivers: The list of receivers.
    pdfPath: The path to the PDF file that is to be sent.
    """

    message = MIMEMultipart()
    message['From'] = senderMail
    message['To'] = '. '.join(receivers)
    message['Subject'] = 'Chronologischer Foto Report.'
    message.attach(MIMEText('Im Anhang befindet sich der Report.', 'plain'))

    fileStream = open(pdfPath, 'rb')
    attach = MIMEApplication(fileStream.read(), _subtype="pdf")
    attach.add_header('Content-Disposition', 'attachment', filename=str(pdfPath))
    message.attach(attach)

    return message


def SendImageMail(emailAddress, password, receivers, sourceDirectory):
    """
    This method can be used to send an E-Mail via the G-Mail API containing an image attachment.

    emailAddress: The E-Mail address from which the mail is sent. Note: This has to be a Gmail address.
    password: The password associated with the email address.
    receivers: List containing E-Mail addresses of receivers.
    sourceDirectory: Path to the directory containing the images that need to be sent.
    """

    message = BuildMessage(emailAddress, receivers, sourceDirectory)
    print('Message with images built. About to send mail.')
    SendMail(emailAddress, password, receivers, message)


def SendPdfMail(emailAddress, password, receivers, pdf):
    """
    This method can be used to send an E-Mail via the G-Mail API containing a PDF attachment.

    emailAddress: The Gmail address from which the mail is sent.
    password: The password associated with the email address.
    receivers: List containing E-Mail addresses of receivers.
    pdf: Path to the PDF that needs to be sent.
    """

    message = BuildMessagePDF(emailAddress, receivers, pdf)
    print('Message with pdf built. About to send mail.')
    SendMail(emailAddress, password, receivers, message)


def SendMail(emailAddress, password, receivers, message):
    """
    This method sends an E-Mail via the Gmail API.

    emailAddress: The E-Mail this mail is sent from.
    password: The password associated with this address.
    receivers: The list of receivers this message is to be sent to.
    message: The formatted E-Mail message that needs to be sent.
    """

    with smtplib.SMTP('smtp.gmail.com:587') as smtp:
        smtp.starttls()
        smtp.login(emailAddress, password)
        for receiver in receivers:
            smtp.sendmail(emailAddress, receiver, message.as_string())
            print('Send to: ' + receiver)
        smtp.quit()
