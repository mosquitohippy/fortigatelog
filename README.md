# fortigatelog

C#

Change format of Fortigate Log from key1=value1 to csv

Well this is my first thing in GitHub ever.

This is a project was assigned to because the FortiGate create a logfile in the format key=value separated with spaces whch was a problem to parse into a Excel File for filtering and such.

A problem I faced was the fact that there were fields enclosed in quotation marks and spaces within so simply using split won't work.

I used a function i found in internet to find spaces outside of the quotation marks and then substitute them with other character, i used the "%".  Once done that i was able to use the split to separate every pair of key/value in the log.  I used a list variable to store it and then again with split i separate the key from the value and store then in a dictionary variable.

Because i didn't know which and how many fields in files as large as 400.000 records i had, i was forced to create 2 files one for the detail and one for the header.  The header was only completed after reading all records because i don't know how many fields were there.

After that i tryed to simply execute a command line copy appending the detail to the header in a different file as is perfectly possible in the commnad line with the comand COPY. I was not able to achieve it so i had to use other loop to copy registry by registry at the end of the new file.

Other thing i had to do was, in order to give the user some rudimentary feedback, create a record counter, to do this i used the backgroundworker.

Hope this little program can help someone.
