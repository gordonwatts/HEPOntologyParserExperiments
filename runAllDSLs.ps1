# Run on all the DSL files in the PaperDSLs directory, produce output owl files.
# We use this in the build CI. That way we can avoid having to escape the '"'.

ls .\PaperDSLs\*.txt | % {.\ParseSingleDFS\bin\Release\ParseSingleDFS.exe $_ > $_.Name.Replace(".txt", ".ttl")}
