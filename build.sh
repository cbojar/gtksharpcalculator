#!/usr/bin/env bash
mono-csc -debug+ -pkg:gtk-sharp-3.0 -out:bin/GTKSharpCalculator.exe src/*
