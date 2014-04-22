mssql-csv-export
================

A windows commandline mssql export tool, written in c#

I started this project because i didn't like the default import and export tool for the mssql database, included in the management studio.

This is a very simple program that extracts all data from your database, and writes it into csv files. The default tool also has the option to export to csv(flat file), but you only could do one table at a time, and my database had like 260 tables. 

As this tool exports the data to csv, all functional database properties like procedures get lost.
