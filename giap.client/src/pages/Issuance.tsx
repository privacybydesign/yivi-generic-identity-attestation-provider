import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useNavigate, useParams} from "react-router";

const yivi = require('@privacybydesign/yivi-frontend'); // this only works with CommonJS, so we use 'require'

interface IdentityProvider {
    name: string;
    slug: string;
    attributes: Record<string, string>;
}

function Issuance() {
    const [identityProvider, setIdentityProvider] = useState<IdentityProvider>();
    const [yiviInitialized, setYiviInitialized] = useState<boolean>(false);
    const [error, setError] = useState<boolean>(false);
    const {t, i18n} = useTranslation();
    let {slug} = useParams();
    const navigate = useNavigate();
    const sessionUrl = `${window.location.origin}/api/identity-provider/${slug}/irma-endpoint`

    useEffect(() => {
        void populateIdentityProviderData(); // https://stackoverflow.com/a/64234381
    }, []);

    useEffect(() => {
        if (identityProvider?.name) {
            document.title = identityProvider.name;
        }
    }, [identityProvider]);

    useEffect(() => {
        if (!identityProvider) return; // Don't show the QR code until the user can see the identity provider data

        if (yiviInitialized) return; // Don't initialize it multiple times
        setYiviInitialized(true);

        const yiviWeb = yivi.newWeb({
            debugging: true,            // Enable to get helpful output in the browser console
            element: '#yivi-web-form', // Which DOM element to render to

            // https://github.com/privacybydesign/yivi-frontend-packages/tree/master/plugins/yivi-web#language
            language: i18n.language == 'en' ? 'en' : 'nl', // Yivi supports either 'en' or 'nl' and defaults to 'nl'

            // Back-end options
            session: {
                // Point this to your controller:
                url: sessionUrl,

                start: {
                    url: (o: { url: string; }) => `${o.url}/start`,
                    method: 'GET'
                },
                result: false
            }
        });
        yiviWeb.start()
            .then(() => navigate(`/${i18n.language}/${slug}/success`), {
                state: {identityProviderName: identityProvider.name}
            })
            .catch((error: any) => console.error("Couldn't do what you asked 😢", error));
    }, [identityProvider, yiviInitialized]);

    if (error) {
        return <div id="container">
            <main id="main-content">
                <p>{t("backendError")}</p>
            </main>
        </div>
    }

    if (!identityProvider) {
        return (
            <div id="container">
                <main id="main-content">
                    <p>{t("loadingContent")}</p>
                </main>
            </div>
        );
    }

    return (
        <>
            <div id="container">
                <header>
                    <h1>{identityProvider.name}</h1>
                </header>
                <main id="main-content">
                    <p>{t("loadAttributesInfo")}</p>
                    <table>
                        <thead>
                        <tr>
                            <th>{t("attributes")}</th>
                            <th>{t("values")}</th>
                        </tr>
                        </thead>
                        <tbody>
                        {Object.entries(identityProvider.attributes).map(([key, value]) => (
                            <tr key={key}>
                                <td>{key}</td>
                                <td>{value}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                    <div id="yivi-web-form"></div>
                </main>
            </div>
        </>
    );

    async function populateIdentityProviderData() {
        try {
            const response = await fetch(`/api/identity-provider/${slug}/attributes/${i18n.language}`);
            if (response.ok) {
                const data = await response.json();
                setIdentityProvider(data);
            } else {
                // Server responds with errors
                setError(true);
            }
        } catch (e) {
            // Server is unreachable
            setError(true);
        }
    }
}

export default Issuance;