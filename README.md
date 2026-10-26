# Kit

Kit is an email marketing platform for managing subscribers, broadcasts, and audience communications.

The Blackbird Kit app enables you to automate subscriber management, create and update email broadcasts, and build multilingual email workflows using Blackbird and Blacklake.

## Before setting up

Before connecting Kit to Blackbird, make sure that:

- You have an active Kit account.
- You have access to create and manage API keys in Kit.
- Your Kit API key has the permissions needed for the operations you intend to automate.

The app uses the Kit API v4 and authenticates using an API key.

## Connecting

1. In Kit, generate or retrieve your API key.
2. In Blackbird, open the Kit app and select **Add connection**.
3. Enter a recognizable connection name.
4. Enter your Kit API key.
5. Select **Connect** and confirm that the connection is established.

Your API key is treated as a sensitive connection credential.

## Actions

### Subscribers

Manage subscriber records in Kit, including creating, retrieving, updating, and searching subscribers.

### Broadcasts

- **Search broadcasts.** Find existing broadcasts in Kit.
- **Get broadcast.** Retrieve broadcast details using its ID.
- **Create broadcast.** Create a broadcast using HTML content.
- **Update broadcast.** Update an existing broadcast.
- **Delete broadcast.** Delete a broadcast.
- **Download broadcast.** Download broadcast content as interoperable HTML.
- **Create localized broadcast.** Create a new draft broadcast from translated HTML or XLIFF.
- **Update localized broadcast.** Update an existing localized draft using translated HTML or XLIFF.

### Multilingual broadcast workflows

The Kit app supports multilingual email workflows using Blackbird's interoperable content format.

A typical workflow is:

1. Download an existing Kit broadcast.
2. Convert its content to XLIFF and translate it using a translation provider.
3. Create a localized broadcast as a separate draft in Kit.
4. Optionally send the translated content to Blacklake for multilingual content tracking.
5. Update the existing localized draft when further translations or revisions are required.

The downloadable content includes metadata identifying the source broadcast and its system. Broadcast subjects and preview text are included as translatable content alongside the email body.

When creating a localized broadcast, Blackbird creates a new draft rather than overwriting the original broadcast.

When updating a localized broadcast, the app uses the specified localized broadcast ID and only permits updates to drafts. For XLIFF files, it validates source and target references to help prevent accidental updates to the original broadcast.

Localized actions return updated content metadata, including the Kit target broadcast reference, for use in subsequent Blackbird and Blacklake workflows.

**Important:** Creating a localized broadcast creates a new Kit record. Use **Update localized broadcast** with the existing localized broadcast ID for subsequent revisions to avoid creating unnecessary duplicate drafts.

## Events

### Broadcasts

- **On broadcast published.** Poll Kit for newly published broadcasts.

### Subscribers

- **On subscriber created.** Poll for newly created subscribers.
- **On subscriber unsubscribed.** Poll for subscriber unsubscribe activity.

These events can be used to initiate automated Birds, including audience synchronization, notifications, and multilingual content workflows.

## Additional notes

- Kit broadcasts created through localization workflows remain drafts until published in Kit.
- The original broadcast and localized broadcasts have different Kit IDs.
- Broadcast HTML is processed for translation interoperability while retaining the underlying email content.
- API availability, permissions, and rate limits are subject to Kit's platform restrictions.

## Feedback

If you encounter an issue or have a suggestion for improving the Kit integration, contact the Blackbird team through the established support channels.
