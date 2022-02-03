$Changes = @()
$Changes += git status docs/ --porcelain

if ($Changes.count -gt 0)
{
	Exit 1
}
else
{
    Exit 0
}