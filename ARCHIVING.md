# Archiving Projects into Avans

This document explains how to merge standalone project repos into this archive while preserving full git history and contributions.

## Why subtree?

Using `git subtree add` pulls an entire repo's commit history into a subdirectory of this repo. All original commits, authors, and timestamps stay intact — nothing gets squashed or lost.

## Single repo

Use this when a project lives in one repo (e.g. `DanielvG-Avans/Q3-Performance-Efficiency`).

```bash
# 1. Add the repo as a temporary remote
git remote add <alias> "<relative or absolute path / remote URL>"

# 2. Fetch it
git fetch <alias>

# 3. Merge it into the target subdirectory
git subtree add --prefix="<Leerjaar X/Project Name>" <alias>/main

# 4. Remove the temporary remote
git remote remove <alias>
```

**Example — Q3 Performance Efficiency:**
```bash
git remote add q3 "../Q3 Performance Efficiency"
git fetch q3
git subtree add --prefix="Leerjaar 2/Q3 Performance Efficiency" q3/main
git remote remove q3
```

## Multi-repo project

Use this when a project is split across multiple repos (e.g. Q4 Softwarearchitectuur with `Docs`, `openmrs-notification-service`, `Softwarekwaliteit`, etc.). Repeat the single-repo steps for each one, targeting a subdirectory per repo.

```bash
git remote add <alias> "<path to subrepo>"
git subtree add --prefix="<Leerjaar X/Project Name/Subrepo Name>" <alias>/main
git remote remove <alias>
```

**Example — Q4 Softwarearchitectuur:**
```bash
git remote add q4-docs "../Q4 Softwarearchitectuur/Docs"
git subtree add --prefix="Leerjaar 2/Q4 Softwarearchitectuur/Docs" q4-docs/main
git remote remove q4-docs

git remote add q4-openmrs "../Q4 Softwarearchitectuur/openmrs-notification-service"
git subtree add --prefix="Leerjaar 2/Q4 Softwarearchitectuur/openmrs-notification-service" q4-openmrs/main
git remote remove q4-openmrs

# repeat for remaining subrepos...
```

## Opening a PR (recommended)

Rather than pushing directly to `main`, archive via a PR so there's a clear record of when a project was merged.

```bash
# 1. Do the subtree add(s) on main locally, then save to a branch
git checkout -b archive/<project-name>

# 2. Reset main back to origin
git checkout main
git reset --hard origin/main

# 3. Push the archive branch and open a PR
git push origin archive/<project-name>
```

Use the branch naming convention `archive/<project-name>`, e.g. `archive/q3-performance-efficiency`.

## Merging the PR

When merging on GitHub, always use **Rebase and merge** — never Squash and merge.

Squash flattens all commits into one, which defeats the purpose of using `git subtree` to preserve history. Rebase keeps every individual commit intact so contributions show up correctly on your GitHub profile.

## Branch naming

| Type | Convention | Example |
|------|-----------|---------|
| Single repo | `archive/<project-name>` | `archive/q3-performance-efficiency` |
| Multi-repo project | `archive/<project-name>` | `archive/q4-softwarearchitectuur` |

## Notes

- If the default branch isn't `main`, swap it for `master` or check with `git branch` inside the source repo.
- The source repo can be referenced by local path (`"../RepoName"`) or remote URL (`https://github.com/org/repo`).
- Once archived here, the original repo in `DanielvG-Avans` can safely be removed — all history lives here.
