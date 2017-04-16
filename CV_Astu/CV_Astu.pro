#-------------------------------------------------
#
# Project created by QtCreator 2017-02-18T17:18:06
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = CV_Astu
TEMPLATE = app

# The following define makes your compiler emit warnings if you use
# any feature of Qt which as been marked as deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0


SOURCES += main.cpp\
        mainwindow.cpp \
    cvimage.cpp \
    doublemat.cpp \
    kernelbuilder.cpp \
    OctaveLayer.cpp \
    pyramid.cpp \
    octave.cpp \
    interestingpointsdetector.cpp \
    descriptorsbuilder.cpp \
    imagehelper.cpp \
    homographyhelper.cpp

HEADERS  += mainwindow.h \
    cvimage.h \
    doublemat.h \
    kernelbuilder.h \
    OctaveLayer.h \
    pyramid.h \
    octave.h \
    interestingpointsdetector.h \
    descriptorsbuilder.h \
    imagehelper.h \
    homographyhelper.h

FORMS    += mainwindow.ui



unix|win32: LIBS += -L$$PWD/gslLib/ -lgsl

INCLUDEPATH += $$PWD/gsl
DEPENDPATH += $$PWD/gsl

unix|win32: LIBS += -L$$PWD/gslLib/ -lgslcblas

INCLUDEPATH += $$PWD/gsl
DEPENDPATH += $$PWD/gsl
